using Egoal.Dependency;
using Egoal.Events.Bus.Handlers;
using Egoal.Payment;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class PaySuccessEventHandler : IAsyncEventHandler<PaySuccessEventData>, IScopedDependency
    {
        private readonly PayOrderAppService _payOrderAppService;

        public PaySuccessEventHandler(PayOrderAppService payOrderAppService)
        {
            _payOrderAppService = payOrderAppService;
        }

        public async Task HandleEventAsync(PaySuccessEventData eventData)
        {
            if (eventData.Attach != NetPayAttach.BuyTicket)
            {
                return;
            }

            await _payOrderAppService.PayOrderAsync(eventData.ListNo, eventData.PayTypeId);
        }
    }
}
