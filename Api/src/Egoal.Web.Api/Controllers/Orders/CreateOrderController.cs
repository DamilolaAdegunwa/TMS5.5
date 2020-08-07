using Egoal.Authorization;
using Egoal.Mvc.Authorization;
using Egoal.Orders;
using Egoal.Orders.Dto;
using Egoal.Scenics;
using Egoal.Staffs;
using Egoal.TicketTypes;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Orders
{
    public class CreateOrderController : TmsControllerBase
    {
        private readonly CreateOrderAppService _createOrderAppService;

        public CreateOrderController(CreateOrderAppService createOrderAppService)
        {
            _createOrderAppService = createOrderAppService;
        }

        [Route("/Api/Order/CreateHandsetOrderAsync")]
        [HttpPost]
        [PermissionFilter(Permissions.Handset_SaleTicket)]
        public async Task<JsonResult> CreateHandsetOrderAsync(CreateOrderInput input)
        {
            var result = await _createOrderAppService.CreateOrderAsync(input, SaleChannel.Local, OrderType.手持机订票);

            return Json(result);
        }

        [Route("/Api/Order/CreateWeiXinOrderAsync")]
        [HttpPost]
        public async Task<JsonResult> CreateWeiXinOrderAsync(CreateOrderInput input)
        {
            input.CashierId = DefaultStaff.微信购票;
            input.CashPcid = DefaultPc.微信购票;
            input.SalePointId = DefaultSalePoint.微信购票;
            input.ParkId = DefaultPark.微信购票;

            var result = await _createOrderAppService.CreateOrderAsync(input, SaleChannel.Net, OrderType.微信订票);

            return Json(result);
        }

        [Route("/Api/Order/CreateSelfHelpOrderAsync")]
        [HttpPost]
        public async Task<JsonResult> CreateSelfHelpOrderAsync(CreateOrderInput input)
        {
            var result = await _createOrderAppService.CreateOrderAsync(input, SaleChannel.Net, OrderType.自助机订票);

            return Json(result);
        }
    }
}
