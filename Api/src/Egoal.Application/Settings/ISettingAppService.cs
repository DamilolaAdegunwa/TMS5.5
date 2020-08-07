using Egoal.Settings.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Settings
{
    public interface ISettingAppService
    {
        Task ConfigOptionsAsync();
        Task<OrderNoticeDto> GetOrderNoticeAsync();
        Task<Dictionary<string, string>> GetSettingsFromWeChatAsync();
        WxJsApiSignature GetWxJsApiSignature(string url);
        string GetWxLoginUrl(string url);
        Task<SelfHelpSettingDto> GetSelfHelpSettingAsync();
        Task EditSelfHelpSettingAsync(SelfHelpSettingDto selfHelpSettingDto);
    }
}
