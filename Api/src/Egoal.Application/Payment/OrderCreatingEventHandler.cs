using Egoal.Dependency;
using Egoal.Events.Bus;
using Egoal.Events.Bus.Entities;
using Egoal.Events.Bus.Handlers;
using Egoal.Orders;
using Egoal.Payment.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public class OrderCreatingEventHandler : IAsyncEventHandler<EntityCreatingEventData<Order>>, IScopedDependency
    {
        private readonly IEventBus _eventBus;
        private readonly IPayAppService _payAppService;

        public OrderCreatingEventHandler(
            IEventBus eventBus,
            IPayAppService payAppService)
        {
            _eventBus = eventBus;
            _payAppService = payAppService;
        }

        public async Task HandleEventAsync(EntityCreatingEventData<Order> eventData)
        {
            var order = eventData.Entity;

            if (order.IsFree())
            {
                var paySuccessEventData = new PaySuccessEventData();
                paySuccessEventData.ListNo = order.Id;
                paySuccessEventData.PayTypeId = DefaultPayType.现金;
                paySuccessEventData.Attach = NetPayAttach.BuyTicket;
                await _eventBus.TriggerAsync(paySuccessEventData);
            }
            else
            {
                await PrePayAsync(eventData.Entity);
            }
        }

        private async Task PrePayAsync(Order order)
        {
            var prePayInput = new PrePayInput();
            prePayInput.ListNo = order.Id;
            prePayInput.PayMoney = order.TotalMoney;
            prePayInput.ProductInfo = order.GetProductInfo();
            prePayInput.ProductId = order.OrderDetails?.FirstOrDefault()?.TicketTypeId?.ToString() ?? "0";
            prePayInput.UserId = order.MemberId?.ToString();
            prePayInput.Attach = NetPayAttach.BuyTicket;

            await _payAppService.PrePayAsync(prePayInput);
        }
    }
}
