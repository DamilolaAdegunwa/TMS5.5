using Egoal.Application.Services;
using Egoal.BackgroundJobs;
using Egoal.Caches;
using Egoal.Cryptography;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.DynamicCodes;
using Egoal.Extensions;
using Egoal.Orders.Dto;
using Egoal.Runtime.Session;
using Egoal.Threading.Lock;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Egoal.Orders
{
    public class CancelOrderAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly IHostingEnvironment _environment;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDistributedLockFactory _lockFactory;
        private readonly INameCacheService _nameCacheService;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly RefundTicketAppService _refundTicketAppService;
        private readonly IOrderDomainService _orderDomainService;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<OrderDetail, long> _orderDetailRepository;
        private readonly IRepository<RefundOrderApply, long> _refundOrderApplyRepository;
        private readonly IBackgroundJobService _backgroundJobService;

        public CancelOrderAppService(
            ISession session,
            IHostingEnvironment environment,
            IUnitOfWorkManager unitOfWorkManager,
            IDistributedLockFactory lockFactory,
            INameCacheService nameCacheService,
            ITicketTypeRepository ticketTypeRepository,
            ITicketSaleRepository ticketSaleRepository,
            IDynamicCodeService dynamicCodeService,
            RefundTicketAppService refundTicketAppService,
            IOrderDomainService orderDomainService,
            IOrderRepository orderRepository,
            IRepository<OrderDetail, long> orderDetailRepository,
            IRepository<RefundOrderApply, long> refundOrderApplyRepository,
            IBackgroundJobService backgroundJobAppService)
        {
            _session = session;
            _environment = environment;
            _unitOfWorkManager = unitOfWorkManager;
            _lockFactory = lockFactory;
            _nameCacheService = nameCacheService;
            _ticketTypeRepository = ticketTypeRepository;
            _ticketSaleRepository = ticketSaleRepository;
            _dynamicCodeService = dynamicCodeService;
            _refundTicketAppService = refundTicketAppService;
            _orderDomainService = orderDomainService;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _refundOrderApplyRepository = refundOrderApplyRepository;
            _backgroundJobService = backgroundJobAppService;
        }

        public async Task CancelByUserAsync(CancelOrderInput input)
        {
            using (var locker = await _lockFactory.LockAsync(ListNo.GetLockKey(input.ListNo)))
            {
                if (!locker.IsAcquired)
                {
                    throw new UserFriendlyException($"订单：{input.ListNo}锁定失败");
                }

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    await CancelOrderAsync(input);

                    await uow.CompleteAsync();
                }
            }
        }

        public async Task CancelOrderAsync(CancelOrderInput input)
        {
            var order = await _orderRepository.GetAllIncluding(o => o.OrderDetails).Where(o => o.Id == input.ListNo).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new UserFriendlyException($"订单{input.ListNo}不存在");
            }

            if (order.OrderStatusId == OrderStatus.已取消)
            {
                return;
            }

            if (order.OrderStatusId == OrderStatus.已完成)
            {
                throw new UserFriendlyException("订单已完成不能取消");
            }

            if (order.HasPaid())
            {
                throw new UserFriendlyException("订单已付款，请申请退款");
            }

            var ticketSales = await _ticketSaleRepository.GetAll()
                .AsNoTracking()
                .Where(t => t.OrderListNo == input.ListNo)
                .ToListAsync();
            if (ticketSales.IsNullOrEmpty())
            {
                await _orderDomainService.CancelAsync(order);

                OrderStat orderStat = new OrderStat();
                orderStat.Cdate = order.Etime;
                orderStat.OrderNum = -order.SurplusNum;
                await _backgroundJobService.EnqueueAsync<UpdateOrderStatJob>(orderStat.ToJson());

                return;
            }

            foreach (var ticketSale in ticketSales)
            {
                ticketSale.ValidateCancelOrder();
            }

            var refundTicketInput = new RefundTicketInput();
            refundTicketInput.OriginalTradeId = ticketSales.FirstOrDefault().TradeId;
            refundTicketInput.RefundListNo = await _dynamicCodeService.GenerateListNoAsync(ListNoType.门票网上订票);
            refundTicketInput.PayListNo = order.Id;
            refundTicketInput.PayTypeId = order.PayTypeId.Value;
            refundTicketInput.RefundReason = "取消订单";
            refundTicketInput.CashierId = _session.StaffId;
            refundTicketInput.CashierName = _nameCacheService.GetStaffName(_session.StaffId);
            refundTicketInput.CashPcid = _session.PcId;
            refundTicketInput.CashPcname = _nameCacheService.GetPcName(_session.PcId);
            refundTicketInput.SalePointId = _session.SalePointId;
            refundTicketInput.SalePointName = _nameCacheService.GetSalePointName(_session.SalePointId);
            refundTicketInput.ParkId = _session.ParkId;
            refundTicketInput.ParkName = _nameCacheService.GetParkName(_session.ParkId);
            foreach (var ticketSale in ticketSales)
            {
                RefundTicketItem refundTicketItem = new RefundTicketItem();
                refundTicketItem.TicketId = ticketSale.Id;
                refundTicketItem.RefundQuantity = ticketSale.PersonNum.Value;
                refundTicketItem.SurplusQuantityAfterRefund = 0;

                refundTicketInput.Items.Add(refundTicketItem);
            }

            await _refundTicketAppService.RefundAsync(refundTicketInput);
        }

        public async Task ApplyRefundAsync(RefundOrderInput input)
        {
            decimal refundMoney = 0;

            foreach (var detail in input.Details)
            {
                var orderDetail = await _orderDetailRepository.FirstOrDefaultAsync(detail.Id);
                if (orderDetail.SurplusNum < detail.RefundQuantity)
                {
                    throw new UserFriendlyException("可退票数不足");
                }

                var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == orderDetail.TicketTypeId);
                if (!ticketType.AllowPartialRefund && detail.RefundQuantity != orderDetail.TotalNum)
                {
                    throw new UserFriendlyException("只能整单取消");
                }

                refundMoney += orderDetail.ReaPrice.Value * detail.RefundQuantity;
            }

            var order = await _orderRepository.FirstOrDefaultAsync(input.ListNo);
            order.RefundStatus = RefundStatus.退款中;

            var refundApply = new RefundOrderApply();
            refundApply.RefundListNo = await _dynamicCodeService.GenerateListNoAsync(ListNoType.门票网上订票);
            refundApply.ListNo = input.ListNo;
            refundApply.RefundQuantity = input.Details.Sum(d => d.RefundQuantity);
            refundApply.RefundMoney = refundMoney;
            refundApply.Details = input.Details.ToJson();
            refundApply.Reason = input.Reason;
            refundApply.CashierId = _session.StaffId;
            refundApply.CashPcid = _session.PcId;
            refundApply.SalePointId = _session.SalePointId;
            refundApply.ParkId = _session.ParkId;
            await _refundOrderApplyRepository.InsertAsync(refundApply);
            await _unitOfWorkManager.Current.SaveChangesAsync();

            var delay = _environment.IsDevelopment() ? 1 : RandomHelper.CreateRandomNumber(20, 300);
            await _backgroundJobService.EnqueueAsync<RefundOrderJob>(refundApply.Id.ToString(), delay: TimeSpan.FromSeconds(delay));
        }
    }
}
