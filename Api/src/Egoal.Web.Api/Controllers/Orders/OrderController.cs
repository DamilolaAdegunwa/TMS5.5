using Egoal.Authorization;
using Egoal.Extensions;
using Egoal.Models;
using Egoal.Mvc.Authorization;
using Egoal.Mvc.Uow;
using Egoal.Orders;
using Egoal.Orders.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Orders
{
    public class OrderController : TmsControllerBase
    {
        private readonly Runtime.Session.ISession _session;
        private readonly QueryOrderAppService _queryOrderAppService;

        public OrderController(
            Runtime.Session.ISession session,
            QueryOrderAppService orderQueryAppService)
        {
            _session = session;
            _queryOrderAppService = orderQueryAppService;
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberOrdersForMobileAsync(GetMemberOrdersForMobileInput input)
        {
            input.MemberId = _session.MemberId.Value;
            input.CustomerId = _session.CustomerId;
            var orders = await _queryOrderAppService.GetMemberOrdersForMobileAsync(input);

            return Json(orders);
        }

        [HttpGet]
        public async Task<JsonResult> GetMemberOrderForMobileAsync(string listNo)
        {
            var result = await _queryOrderAppService.GetMemberOrderForMobileAsync(listNo);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetLastOrderFullInfoAsync(GetLastOrderInput input)
        {
            var result = await _queryOrderAppService.GetLastOrderFullInfoAsync(input);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderFullInfoAsync(GetOrderInfoInput input)
        {
            var order = await _queryOrderAppService.GetOrderFullInfoAsync(input);

            return Json(order);
        }

        [HttpGet]
        public async Task<JsonResult> GetRefundApplysWithStatusDetailAsync(string listNo)
        {
            var result = await _queryOrderAppService.GetRefundApplysWithStatusDetailAsync(listNo);

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderOptionsAsync()
        {
            var options = await _queryOrderAppService.GetOrderOptionsAsync();

            return Json(options);
        }

        [HttpPost]
        public async Task<JsonResult> GetGroupOrdersForConsumeAsync(GetGroupOrdersForConsumeInput input)
        {
            var orders = await _queryOrderAppService.GetGroupOrdersForConsumeAsync(input);

            return Json(orders);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_OrderManage)]
        public async Task<FileContentResult> GetOrdersToExcelAsync([FromForm]GetOrdersInput input)
        {
            var fileContents = await _queryOrderAppService.GetOrdersToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_OrderManage)]
        public async Task<JsonResult> GetOrdersAsync([FromForm]GetOrdersInput input)
        {
            var result = await _queryOrderAppService.GetOrdersAsync(input);

            return Json(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [UnitOfWork(false)]
        public async Task<JsonResult> StatOrderByCustomerAsync(StatOrderByCustomerInput input)
        {
            var result = await _queryOrderAppService.StatOrderByCustomerAsync(input);

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetOrderStatusComboboxItems()
        {
            var items = typeof(OrderStatus).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetConsumeStatusComboboxItems()
        {
            var items = typeof(ConsumeStatus).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetRefundStatusComboboxItems()
        {
            var items = typeof(RefundStatus).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetOrderTypeComboboxItems()
        {
            var items = typeof(OrderType).ToComboboxItems();

            return Json(items);
        }

        [HttpPost]
        public async Task<JsonResult> GetSelfHelpTicketGroundAsync(SelfHelpTicketGroundInput input)
        {
            var result = await _queryOrderAppService.GetSelfHelpTicketGroundAsync(input);

            return Json(result);
        }

        [HttpPost]
        public async Task<JsonResult> GetSelfHelpOrderTicketAsync(SelfHelpOrderTicketInput input)
        {
            var result = await _queryOrderAppService.GetSelfHelpOrderTicketAsync(input);

            return Json(result);
        }
    }
}