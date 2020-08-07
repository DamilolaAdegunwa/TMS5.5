using Egoal.Dependency;
using Egoal.Events.Bus.Entities;
using Egoal.Events.Bus.Handlers;
using Egoal.Orders;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public class OrderDeletingEventHandler : IAsyncEventHandler<EntityDeletingEventData<Order>>, IScopedDependency
    {
        private readonly IPayAppService _payAppService;

        public OrderDeletingEventHandler(IPayAppService payAppService)
        {
            _payAppService = payAppService;
        }

        public async Task HandleEventAsync(EntityDeletingEventData<Order> eventData)
        {
            await _payAppService.ClosePayAsync(eventData.Entity.Id);
        }
    }
}
