using Egoal.Dependency;
using Egoal.Events.Bus.Handlers;
using Egoal.Payment;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class PayTimeoutEventHandler : IAsyncEventHandler<PayTimeoutEventData>, IScopedDependency
    {
        private readonly CancelOrderAppService _cancelOrderAppService;

        public PayTimeoutEventHandler(CancelOrderAppService cancelOrderAppService)
        {
            _cancelOrderAppService = cancelOrderAppService;
        }

        public async Task HandleEventAsync(PayTimeoutEventData eventData)
        {
            if (eventData.Attach != NetPayAttach.BuyTicket)
            {
                return;
            }

            await _cancelOrderAppService.CancelOrderAsync(new Dto.CancelOrderInput { ListNo = eventData.ListNo });
        }
    }
}
