using Egoal.Application.Services;
using Egoal.BackgroundJobs;
using Egoal.Caches;
using Egoal.Cryptography;
using Egoal.DynamicCodes;
using Egoal.Extensions;
using Egoal.Members;
using Egoal.Orders.Dto;
using Egoal.Payment;
using Egoal.Runtime.Session;
using Egoal.Scenics.Dto;
using Egoal.TicketTypes;
using Egoal.UI;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class CreateOrderAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly INameCacheService _nameCacheService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly IOrderDomainService _orderDomainService;
        private readonly ITicketTypeDomainService _ticketTypeDomainService;
        private readonly IOrderRepository _orderRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;

        public CreateOrderAppService(
            ISession session,
            IDynamicCodeService dynamicCodeService,
            INameCacheService nameCacheService,
            IBackgroundJobService backgroundJobService,
            IOrderDomainService orderDomainService,
            ITicketTypeDomainService ticketTypeDomainService,
            IOrderRepository orderRepository,
            ITicketTypeRepository ticketTypeRepository)
        {
            _session = session;
            _dynamicCodeService = dynamicCodeService;
            _nameCacheService = nameCacheService;
            _backgroundJobService = backgroundJobService;
            _orderDomainService = orderDomainService;
            _ticketTypeDomainService = ticketTypeDomainService;
            _orderRepository = orderRepository;
            _ticketTypeRepository = ticketTypeRepository;
        }

        public async Task<CreateOrderOutput> CreateOrderAsync(CreateOrderInput input, SaleChannel saleChannel, OrderType orderType)
        {
            CheckSign(input);

            var order = new Order();
            order.Id = await _dynamicCodeService.GenerateListNoAsync(saleChannel == SaleChannel.Local ? ListNoType.门票 : ListNoType.门票网上订票);
            order.OrderTypeId = orderType;
            order.OrderTypeName = orderType.ToString();
            order.Etime = input.TravelDate.ToDateString();
            order.PaymentMethod = saleChannel == SaleChannel.Local ? PaymentMethod.现场支付 : PaymentMethod.在线支付;
            if (orderType == OrderType.微信订票)
            {
                order.MemberId = _session.MemberId;
                order.MemberName = _nameCacheService.GetMemberName(order.MemberId);
            }
            order.YdrName = order.JidiaoName = input.ContactName;
            order.Mobile = order.JidiaoMobile = input.ContactMobile;
            order.CertTypeId = input.ContactCertTypeId;
            string certTypeName = input.ContactCertTypeId.HasValue ? DefaultCertType.GetName(input.ContactCertTypeId.Value) : "";
            if (string.IsNullOrEmpty(certTypeName) && input.ContactCertTypeId.HasValue)
            {
                certTypeName = _nameCacheService.GetCertTypeName(input.ContactCertTypeId);
            }
            order.CertTypeName = certTypeName;
            order.CertNo = input.ContactCert?.ToUpper();
            order.PromoterId = input.PromoterId;
            order.CashierId = input.CashierId;
            order.CashPcId = input.CashPcid;
            order.SalePointId = input.SalePointId;
            order.ParkId = input.ParkId;
            order.ParkName = _nameCacheService.GetParkName(order.ParkId);

            foreach (var item in input.Items)
            {
                var ticketType = await _ticketTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(t => t.Id == item.TicketTypeId);

                var orderDetail = order.MapToOrderDetail();
                orderDetail.SetTicketType(ticketType);
                orderDetail.TotalNum = orderDetail.SurplusNum = item.Quantity;
                var price = await _ticketTypeDomainService.GetPriceAsync(ticketType, input.TravelDate, saleChannel);
                orderDetail.SetTicMoney(orderDetail.TotalNum, price);
                orderDetail.SetReaMoney(orderDetail.TotalNum, price);

                BuildOrderGroundChangCis(orderDetail, item.GroundChangCis);
                BuildOrderTourists(orderDetail, item.Tourists);

                order.OrderDetails.Add(orderDetail);
            }
            order.StatFlag = order.OrderDetails.Any(o => o.StatFlag == true);

            await _orderDomainService.CreateAsync(order);

            await _orderRepository.InsertAndGetIdAsync(order);

            OrderStat orderStat = new OrderStat();
            orderStat.Cdate = order.Etime;
            orderStat.OrderNum = order.TotalNum;
            await _backgroundJobService.EnqueueAsync<UpdateOrderStatJob>(orderStat.ToJson());

            var output = new CreateOrderOutput();
            output.ListNo = order.Id;
            output.ShouldPay = !order.IsFree();
            return output;
        }

        private void CheckSign(CreateOrderInput input)
        {
            StringBuilder signBuilder = new StringBuilder();
            signBuilder.Append(input.TravelDate.ToDateString());
            var sortedItems = input.Items.OrderBy(i => i.TicketTypeId);
            foreach (var item in sortedItems)
            {
                signBuilder.Append(item.TicketTypeId).Append(item.Quantity);
            }
            signBuilder.Append(CreateOrderInput.SignKey);
            var sign = MD5Helper.Encrypt(signBuilder.ToString());
            if (!sign.Equals(input.Sign, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("无效请求");
            }
        }

        private void BuildOrderGroundChangCis(OrderDetail orderDetail, IEnumerable<GroundChangCiDto> groundChangCis)
        {
            if (groundChangCis.IsNullOrEmpty()) return;

            foreach (var groundChangCi in groundChangCis)
            {
                var orderGroundChangCi = new OrderGroundChangCi();
                orderGroundChangCi.GroundId = groundChangCi.GroundId;
                orderGroundChangCi.ChangCiId = groundChangCi.ChangCiId;

                var quantity = orderDetail.TotalNum;
                if (orderDetail.TicketType != null && orderDetail.TicketType.CheckTypeId == CheckType.家庭套票)
                {
                    quantity = quantity * orderDetail.TicketType.CheckNum.Value;
                }
                orderGroundChangCi.Quantity = quantity;

                orderDetail.AddOrderGroundChangCi(orderGroundChangCi);
            }
        }

        private void BuildOrderTourists(OrderDetail orderDetail, IEnumerable<OrderTouristDto> tourists)
        {
            if (tourists.IsNullOrEmpty()) return;

            foreach (var tourist in tourists)
            {
                var orderTourist = new OrderTourist();
                orderTourist.Name = tourist.Name;
                orderTourist.Mobile = tourist.Mobile;
                orderTourist.CertType = tourist.CertTypeId == null ? DefaultCertType.二代身份证 : tourist.CertTypeId;
                orderTourist.CertNo = tourist.CertNo.ToUpper();

                orderDetail.AddOrderTourist(orderTourist);
            }
        }
    }
}
