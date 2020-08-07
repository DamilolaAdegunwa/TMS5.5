using Egoal.Application.Services.Dto;
using Egoal.Authorization;
using Egoal.Extensions;
using Egoal.Models;
using Egoal.Mvc.Authorization;
using Egoal.ValueCards;
using Egoal.ValueCards.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class ValueCardController : TmsControllerBase
    {
        private readonly IValueCardQueryAppService _valueCardQueryAppService;

        public ValueCardController(IValueCardQueryAppService valueCardQueryAppService)
        {
            _valueCardQueryAppService = valueCardQueryAppService;
        }

        [HttpPost]
        [DontWrapResult]
        [PermissionFilter(Permissions.TMSAdmin_QueryCzkDetail)]
        public async Task<FileContentResult> QueryCzkDetailsToExcelAsync([FromForm]QueryCzkDetailInput input)
        {
            var fileContents = await _valueCardQueryAppService.QueryCzkDetailsToExcelAsync(input);

            return Excel(fileContents);
        }

        [HttpPost]
        [PermissionFilter(Permissions.TMSAdmin_QueryCzkDetail)]
        public async Task<PagedResultDto<CzkDetailListDto>> QueryCzkDetailsAsync([FromForm]QueryCzkDetailInput input)
        {
            return await _valueCardQueryAppService.QueryCzkDetailsAsync(input);
        }

        [HttpGet]
        public JsonResult GetCzkOpTypeComboboxItems()
        {
            var items = typeof(CzkOpType).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetCzkRechargeTypeComboboxItems()
        {
            var items = typeof(CzkRechargeType).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public JsonResult GetCzkConsumeTypeComboboxItems()
        {
            var items = typeof(CzkConsumeType).ToComboboxItems();

            return Json(items);
        }

        [HttpGet]
        public async Task<List<ComboboxItemDto<int>>> GetCzkCztcComboboxItemsAsync()
        {
            return await _valueCardQueryAppService.GetCzkCztcComboboxItemsAsync();
        }
    }
}
