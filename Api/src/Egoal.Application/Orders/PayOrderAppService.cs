using Egoal.Application.Services;
using Egoal.BackgroundJobs;
using Egoal.Caches;
using Egoal.Extensions;
using Egoal.Orders.Dto;
using Egoal.Payment;
using Egoal.Scenics.Dto;
using Egoal.Stadiums;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.Trades;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class PayOrderAppService : ApplicationService
    {
        private readonly INameCacheService _nameCacheService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly CreateTicketAppService _createTicketAppService;
        private readonly IOrderRepository _orderRepository;
        private readonly ISeatStatusCacheRepository _seatStatusCacheRepository;

        public PayOrderAppService(
            INameCacheService nameCacheService,
            IBackgroundJobService backgroundJobService,
            CreateTicketAppService createTicketAppService,
            IOrderRepository orderRepository,
            ISeatStatusCacheRepository seatStatusCacheRepository)
        {
            _nameCacheService = nameCacheService;
            _backgroundJobService = backgroundJobService;
            _createTicketAppService = createTicketAppService;
            _orderRepository = orderRepository;
            _seatStatusCacheRepository = seatStatusCacheRepository;
        }

        public async Task PayOrderAsync(string listNo, int payTypeId)
        {
            var order = await _orderRepository.GetByIdAsync(listNo);

            order.Pay(payTypeId, DefaultPayType.GetName(payTypeId));

            await ChangeTicketAsync(order);

            if (order.OrderTypeId == OrderType.微信订票)
            {
                var message = new PaySuccessMessage();
                message.OrderType = order.OrderTypeId;
                message.MemberId = order.MemberId.Value;
                message.ListNo = order.Id;
                message.TotalMoney = order.TotalMoney;
                message.ProductInfo = order.GetProductInfo();
                await _backgroundJobService.EnqueueAsync<SendPaySuccessMessageJob>(message.ToJson());
            }
        }

        private async Task ChangeTicketAsync(Order order)
        {
            var saleTicketInput = await BuildSaleTicketInputAsync(order);
            var ticketSales = await _createTicketAppService.SaleAsync(saleTicketInput);

            order.EndTime = ticketSales.Max(t => t.Etime.To<DateTime>());
        }

        private async Task<SaleTicketInput> BuildSaleTicketInputAsync(Order order)
        {
            var saleTicketInput = order.MapToSaleTicketInput();
            saleTicketInput.TravelDate = order.Etime.To<DateTime>();
            saleTicketInput.TradeTypeTypeId = TradeTypeType.门票;
            saleTicketInput.TradeTypeId = order.OrderTypeId == OrderType.微信订票 ? DefaultTradeType.门票_微信 : DefaultTradeType.门票;
            saleTicketInput.TradeTypeName = DefaultTradeType.GetName(saleTicketInput.TradeTypeId.Value);
            saleTicketInput.CashierId = order.CashierId;
            saleTicketInput.CashierName = _nameCacheService.GetStaffName(saleTicketInput.CashierId);
            saleTicketInput.CashPcid = order.CashPcId;
            saleTicketInput.CashPcname = _nameCacheService.GetPcName(saleTicketInput.CashPcid);
            saleTicketInput.SalePointId = order.SalePointId;
            saleTicketInput.SalePointName = _nameCacheService.GetSalePointName(saleTicketInput.SalePointId);
            saleTicketInput.ParkId = order.ParkId;
            saleTicketInput.ParkName = _nameCacheService.GetParkName(saleTicketInput.ParkId);
            saleTicketInput.IsExchange = true;

            if (order.OrderDetails.Any(o => o.HasGroundSeat))
            {
                saleTicketInput.Seats = await _seatStatusCacheRepository.GetOrderSeatsAsync(order.Id);
            }

            foreach (var orderDetail in order.OrderDetails)
            {
                var saleTicketItem = new SaleTicketItem();
                saleTicketItem.TicketTypeId = orderDetail.TicketTypeId.Value;
                saleTicketItem.Quantity = orderDetail.TotalNum;
                saleTicketItem.TicPrice = orderDetail.TicPrice.Value;
                saleTicketItem.RealPrice = orderDetail.ReaPrice.Value;
                saleTicketItem.OrderDetailId = orderDetail.Id;
                saleTicketItem.HasGroundSeat = orderDetail.HasGroundSeat;
                saleTicketItem.GroundChangCis = orderDetail.OrderGroundChangCis.Adapt<List<GroundChangCiDto>>();
                saleTicketItem.Tourists = orderDetail.OrderTourists.Adapt<List<TicketTourist>>();

                saleTicketInput.Items.Add(saleTicketItem);
            }

            return saleTicketInput;
        }
    }
}
