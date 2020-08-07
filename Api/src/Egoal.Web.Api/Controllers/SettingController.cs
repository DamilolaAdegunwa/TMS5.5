using Egoal.Settings;
using Egoal.Settings.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class SettingController : TmsControllerBase
    {
        private readonly ISettingAppService _settingAppService;

        public SettingController(ISettingAppService settingAppService)
        {
            _settingAppService = settingAppService;
        }

        [HttpGet]
        public async Task<JsonResult> GetOrderNoticeAsync()
        {
            var result = await _settingAppService.GetOrderNoticeAsync();

            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetSettingsFromWeChatAsync()
        {
            var result = await _settingAppService.GetSettingsFromWeChatAsync();

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetWxJsApiSignature(string url)
        {
            var result = _settingAppService.GetWxJsApiSignature(url);

            return Json(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public string GetWxLoginUrl(string url)
        {
            return _settingAppService.GetWxLoginUrl(url);
        }

        [HttpGet]
        public async Task<JsonResult> GetSelfHelpSettingAsync()
        {
            var result = await _settingAppService.GetSelfHelpSettingAsync();

            return Json(result);
        }

        [HttpPost]
        public async Task EditSelfHelpSettingAsync(SelfHelpSettingDto selfHelpSettingDto)
        {
            await _settingAppService.EditSelfHelpSettingAsync(selfHelpSettingDto);
        }
    }
}
