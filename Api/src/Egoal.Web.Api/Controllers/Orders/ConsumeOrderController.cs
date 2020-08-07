using Egoal.Orders;
using Egoal.Orders.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Orders
{
    public class ConsumeOrderController : TmsControllerBase
    {
        private readonly ConsumeOrderAppService _consumeOrderAppService;

        public ConsumeOrderController(ConsumeOrderAppService consumeOrderAppService)
        {
            _consumeOrderAppService = consumeOrderAppService;
        }

        [Route("/Api/Order/ConsumeOrderFromMobileAsync")]
        [HttpPost]
        public async Task ConsumeOrderFromMobileAsync(ConsumeOrderFromMobileInput input)
        {
            await _consumeOrderAppService.ConsumeOrderFromMobileAsync(input);
        }
    }
}
