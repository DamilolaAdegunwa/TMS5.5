using Egoal.Application.Services.Dto;
using Egoal.Authorization;
using Egoal.Models;
using Egoal.Mvc.Authorization;
using Egoal.Tickets.Dto;
using Egoal.Wares;
using Egoal.Wares.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class WareController : TmsControllerBase
    {
        private readonly IWareQueryAppService _wareQueryAppService;

        public WareController(IWareQueryAppService wareQueryAppService)
        {
            _wareQueryAppService = wareQueryAppService;
        }

        [HttpGet]
        public async Task<JsonResult> GetMerchantComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetMerchantComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetShopComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetShopComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetShopTypeComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetShopTypeComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetSupplierComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetSupplierComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetWareTypeComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetWareTypeComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetWareTypeTypeComboBoxItemsAsync()
        {
            var result = await _wareQueryAppService.GetWareTypeTypeComboBoxItemsAsync();
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetPayDetailStatTypeComboBoxItems()
        {
            var result = _wareQueryAppService.GetPayDetailStatTypeComboBoxItems();
            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWareIODetail)]
        public async Task<JsonResult> QueryWareAsync(QueryWareInput input)
        {
            var result = await _wareQueryAppService.QueryWareAsync(input);
            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWare)]
        public async Task<FileContentResult> QueryWareToExcelAsync(QueryWareInput input)
        {
            var fileContents = await _wareQueryAppService.QueryWareToExcelAsync(input);
            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWareIODetail)]
        public async Task<JsonResult> QueryWareIODetailAsync([FromForm]QueryWareIODetailInput input)
        {
            var result = await _wareQueryAppService.QueryWareIODetailAsync(input);
            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWareIODetail)]
        public async Task<FileContentResult> QueryWareIODetailToExcelAsync([FromForm]QueryWareIODetailInput input)
        {
            var fileContents = await _wareQueryAppService.QueryWareIODetailToExcelAsync(input);
            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWareTrade)]
        public async Task<JsonResult> QueryWareTradeAsync([FromForm]QueryWareTradeInput input)
        {
            var result = await _wareQueryAppService.QueryWareTradeAsync(input);
            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_WareSearchWareTrade)]
        public async Task<FileContentResult> QueryWareTradeToExcelAsync([FromForm]QueryWareTradeInput input)
        {
            var fileContents = await _wareQueryAppService.QueryWareTradeToExcelAsync(input);
            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareTrade)]
        public async Task<JsonResult> StatWareTradeAsync([FromForm]StatWareTradeInput input)
        {
            DynamicColumnResultDto result = await _wareQueryAppService.StatWareTradeAsync(input);
            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareTrade)]
        public async Task<FileContentResult> StatWareTradeToExcelAsync([FromForm]StatWareTradeInput input)
        {
            byte[] fileContents = await _wareQueryAppService.StatWareTradeToExcelAsync(input);
            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareSaleByWareType)]
        public async Task<JsonResult> StatWareSaleByWareTypeAsync([FromForm]StatWareSaleByWareTypeInput input)
        {
            DynamicColumnResultDto result = await _wareQueryAppService.StatWareSaleByWareTypeAsync(input);
            return Json(result);
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareSaleByWareType)]
        public async Task<FileContentResult> StatWareSaleByWareTypeToExcelAsync([FromForm]StatWareSaleByWareTypeInput input)
        {
            byte[] fileContents = await _wareQueryAppService.StatWareSaleByWareTypeToExcelAsync(input);
            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareRentSale)]
        public async Task<JsonResult> StatWareRentSaleAsync([FromForm]StatWareRentSaleInput input)
        {
            DataSet result = await _wareQueryAppService.StatWareRentSaleAsync(input);
            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareTradeTotal)]
        public async Task<JsonResult> StatWareTradeTotalAsync([FromForm]StatWareTradeTotalInput input)
        {
            DataTable result = await _wareQueryAppService.StatWareTradeTotalAsync(input);
            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_WareStatWareSale)]
        public async Task<JsonResult> StatWareSaleAsync([FromForm]StatWareSaleInput input)
        {
            DataTable result = await _wareQueryAppService.StatWareSaleAsync(input);
            return Json(result);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_StatShift)]
        public async Task<JsonResult> StatWareSaleShiftAsync(StatJbInput input)
        {
            DataTable dataTable = await _wareQueryAppService.StatWareSaleShiftAsync(input);
            return Json(dataTable);
        }
    }
}
