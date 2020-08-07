using Egoal.Application.Services.Dto;
using Egoal.Authorization;
using Egoal.Extensions;
using Egoal.Models;
using Egoal.Mvc.Authorization;
using Egoal.Mvc.Uow;
using Egoal.Tickets.Dto;
using Egoal.Trades;
using Egoal.Trades.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class TradeController : TmsControllerBase
    {
        private readonly ITradeAppService _tradeAppService;

        public TradeController(ITradeAppService tradeAppService)
        {
            _tradeAppService = tradeAppService;
        }

        [HttpGet]
        public JsonResult GetTradeSourceComboboxItems()
        {
            var items = typeof(TradeSource).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public async Task<JsonResult> GetTradeTypeTypeComboboxItemsAsync()
        {
            var items = await _tradeAppService.GetTradeTypeTypeComboboxItemsAsync();

            return Json(items);
        }

        [HttpGet]
        public async Task<JsonResult> GetTradeTypeComboboxItemsAsync(int? tradeTypeTypeId)
        {
            var items = await _tradeAppService.GetTradeTypeComboboxItemsAsync(tradeTypeTypeId);

            return Json(items);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_SearchTrade)]
        public async Task<JsonResult> QueryTradesAsync([FromForm]QueryTradeInput input)
        {
            var result = await _tradeAppService.QueryTradesAsync(input);

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetPayDetailStatTypeComboboxItems()
        {
            var items = typeof(PayDetailStatType).ToComboboxItems();

            return Json(items);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatPayDetail)]
        public async Task<FileContentResult> StatPayDetailToExcelAsync([FromForm]StatPayDetailInput input)
        {
            var fileContents = await _tradeAppService.StatPayDetailToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatPayDetail)]
        public async Task<JsonResult> StatPayDetailAsync([FromForm]StatPayDetailInput input)
        {
            var result = await _tradeAppService.StatPayDetailAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatShift, Permissions.TMSWeChat_TradeStat)]
        public async Task<JsonResult> StatPayDetailJbAsync([FromForm]StatJbInput input)
        {
            var result = await _tradeAppService.StatPayDetailJbAsync(input);

            return Json(result);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSWeChat_TradeStat)]
        public async Task<JsonResult> StatTradeAsync([FromForm]StatTradeInput input)
        {
            var result = await _tradeAppService.StatTradeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTradeByPayType)]
        public async Task<FileContentResult> StatTradeByPayTypeToExcelAsync([FromForm]StatTradeByPayTypeInput input)
        {
            var fileContents = await _tradeAppService.StatTradeByPayTypeToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatTradeByPayType)]
        public async Task<JsonResult> StatTradeByPayTypeAsync([FromForm]StatTradeByPayTypeInput input)
        {
            var result = await _tradeAppService.StatTradeByPayTypeAsync(input);

            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatPayDetailByCustomer)]
        public async Task<FileContentResult> StatPayDetailByCustomerToExcelAsync([FromForm]StatPayDetailByCustomerInput input)
        {
            var fileContents = await _tradeAppService.StatPayDetailByCustomerToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [UnitOfWork(false)]
        [PermissionFilter(Permissions.TMSAdmin_StatPayDetailByCustomer)]
        public async Task<DynamicColumnResultDto> StatPayDetailByCustomerAsync([FromForm]StatPayDetailByCustomerInput input)
        {
            return await _tradeAppService.StatPayDetailByCustomerAsync(input);
        }
    }
}
