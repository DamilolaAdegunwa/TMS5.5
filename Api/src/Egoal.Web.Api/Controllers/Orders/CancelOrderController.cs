using Egoal.Mvc.Uow;
using Egoal.Orders;
using Egoal.Orders.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Orders
{
    public class CancelOrderController : TmsControllerBase
    {
        private readonly CancelOrderAppService _cancelOrderAppService;

        public CancelOrderController(CancelOrderAppService cancelOrderAppService)
        {
            _cancelOrderAppService = cancelOrderAppService;
        }

        [Route("/Api/Order/CancelOrderAsync")]
        [HttpPost]
        [UnitOfWork(IsDisabled = true)]
        public async Task CancelOrderAsync(CancelOrderInput input)
        {
            await _cancelOrderAppService.CancelByUserAsync(input);
        }

        [Route("/Api/Order/ApplyRefundAsync")]
        [HttpPost]
        public async Task ApplyRefundAsync(RefundOrderInput input)
        {
            await _cancelOrderAppService.ApplyRefundAsync(input);
        }
    }
}
