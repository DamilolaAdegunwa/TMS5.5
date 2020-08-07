using Egoal.Application.Services;
using Egoal.Authorization;
using Egoal.Cryptography;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.Extensions;
using Egoal.Scenics;
using Egoal.Settings.Dto;
using Egoal.WeChat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Settings
{
    public class SettingAppService : ApplicationService, ISettingAppService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Setting, string> _settingRepository;

        public SettingAppService(
            IServiceProvider serviceProvider,
            ILogger<SettingAppService> logger,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Setting, string> settingRepository)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _unitOfWorkManager = unitOfWorkManager;
            _settingRepository = settingRepository;
        }

        public async Task ConfigOptionsAsync()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                ConfigJwtOptions();
                await ConfigWeChatOptionsAsync();
                await ConfigScenicOptionsAsync();

                await EnsureEmailSettingsExistsAsync();
                await EnsureFaceSettingsExistsAsync();

                await uow.CompleteAsync();
            }
        }

        private void ConfigJwtOptions()
        {
            var tokenService = _serviceProvider.GetRequiredService<ITokenService>();
            tokenService.ConfigJwtOptions();
        }

        private async Task ConfigWeChatOptionsAsync()
        {
            var weChatOptions = _serviceProvider.GetRequiredService<IOptions<WeChatOptions>>().Value;
            var parkOptions = _serviceProvider.GetRequiredService<IOptions<ParkOptions>>().Value;

            weChatOptions.NotifyUrl = parkOptions.WebApiUrl?.UrlCombine("Api", "Payment");

            await EnsureSettingExistsAsync("WxSubscribeUrl", "微信公众号地址");
            await EnsureSettingExistsAsync("WxMenuUrl", "微信购票菜单地址");

            try
            {
                weChatOptions.SslCert = new X509Certificate2(weChatOptions.WxCertPath, weChatOptions.WxCertPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, weChatOptions.WxCertPath);

                weChatOptions.SslCert = LoadCertFromStore(new[] { "O=Tencent", "O=Tenpay.com" });
            }
        }

        private async Task EnsureEmailSettingsExistsAsync()
        {
            await EnsureSettingExistsAsync("SmtpHost", "SMTP服务器");
            await EnsureSettingExistsAsync("SmtpPort", "SMTP端口", "465");
            await EnsureSettingExistsAsync("SmtpUserName", "SMTP用户名");
            await EnsureSettingExistsAsync("SmtpPassword", "SMTP密码");
            await EnsureSettingExistsAsync("SmtpUseSSL", "SMTP使用SSL", "True");
            await EnsureSettingExistsAsync("EmailDefaultFromAddress", "默认发件邮箱地址");
            await EnsureSettingExistsAsync("EmailDefaultFromDisplayName", "默认发件人");
        }

        private async Task EnsureFaceSettingsExistsAsync()
        {
            await EnsureSettingExistsAsync("FaceServerUrl", "人像服务器地址");
        }

        private async Task EnsureSettingExistsAsync(string name, string caption, string defaultValue = "")
        {
            if (!await _settingRepository.AnyAsync(s => s.Id == name))
            {
                var setting = new Setting
                {
                    Id = name,
                    Caption = caption
                };

                if (!defaultValue.IsNullOrEmpty())
                {
                    setting.Value = defaultValue;
                }

                await _settingRepository.InsertAsync(setting);
            }
        }

        private X509Certificate2 LoadCertFromStore(string[] issuers)
        {
            try
            {
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                foreach (var cert in store.Certificates)
                {
                    if (issuers.Any(issuer => cert.Issuer.Contains(issuer)))
                    {
                        return cert;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return null;
            }
        }

        private async Task ConfigScenicOptionsAsync()
        {
            var options = _serviceProvider.GetRequiredService<IOptions<ScenicOptions>>().Value;

            var parkRepository = _serviceProvider.GetRequiredService<IRepository<Park>>();
            options.IsMultiPark = await parkRepository.CountAsync() > 1;

            var scenicRepository = _serviceProvider.GetRequiredService<IRepository<Scenic>>();
            var scenic = await scenicRepository.FirstOrDefaultAsync(s => s.Id > 0);
            if (scenic != null)
            {
                options.ScenicName = scenic.ScenicName;
                options.ParkOpenTime = scenic.OpenTime;
                options.ParkCloseTime = scenic.CloseTime;
            }
            else
            {
                var constantRepository = _serviceProvider.GetRequiredService<IRepository<Constant, string>>();
                options.ScenicName = (await constantRepository.FirstOrDefaultAsync("CompanyName"))?.Value;
            }
        }

        public async Task<OrderNoticeDto> GetOrderNoticeAsync()
        {
            var constantRepository = _serviceProvider.GetRequiredService<IRepository<Constant, string>>();

            var output = new OrderNoticeDto();
            output.ScenicName = (await constantRepository.FirstOrDefaultAsync("CompanyName"))?.Value;
            output.TimeNotice = (await constantRepository.FirstOrDefaultAsync("开馆时间"))?.Value;
            output.OrderNotice = (await constantRepository.FirstOrDefaultAsync("预约须知"))?.Value;
            output.ContactNotice = (await constantRepository.FirstOrDefaultAsync("联系信息"))?.Value;

            return output;
        }

        public async Task<Dictionary<string, string>> GetSettingsFromWeChatAsync()
        {
            var settings = new Dictionary<string, string>();

            var distributionUrl = await _settingRepository.GetAll()
                .Where(s => s.Id == "App.General.DistributionUrl")
                .Select(s => s.Value)
                .FirstOrDefaultAsync();
            settings.Add("DistributionUrl", distributionUrl);

            var wxSubscribeUrl = await _settingRepository.GetAll()
                .Where(s => s.Id == "WxSubscribeUrl")
                .Select(s => s.Value)
                .FirstOrDefaultAsync();
            settings.Add("WxSubscribeUrl", wxSubscribeUrl);

            var wxMenuUrl = await _settingRepository.GetAll()
                .Where(s => s.Id == "WxMenuUrl")
                .Select(s => s.Value)
                .FirstOrDefaultAsync();
            settings.Add("WxMenuUrl", wxMenuUrl);

            return settings;
        }

        public WxJsApiSignature GetWxJsApiSignature(string url)
        {
            var options = _serviceProvider.GetRequiredService<IOptions<WeChatOptions>>().Value;

            var signature = new WxJsApiSignature();
            signature.AppId = options.WxAppID;
            signature.NonceStr = Guid.NewGuid().ToString().Replace("-", string.Empty);
            signature.Timestamp = DateTime.Now.ToUnixTimestamp().To<long>();

            var args = new SortedDictionary<string, string>();
            args.Add("noncestr", signature.NonceStr);
            args.Add("timestamp", signature.Timestamp.ToString());
            args.Add("jsapi_ticket", options.JsApiTicket);
            args.Add("url", url);

            var stringArgs = new StringBuilder();
            foreach (var pair in args)
            {
                stringArgs.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }

            signature.Signature = SHAHelper.SHA1Encrypt(stringArgs.ToString().TrimEnd('&')).ToLower();

            return signature;
        }

        public string GetWxLoginUrl(string url)
        {
            var options = _serviceProvider.GetRequiredService<IOptions<WeChatOptions>>().Value;

            return $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={options.WxAppID}&redirect_uri={url.UrlEncode()}&response_type=code&scope=snsapi_userinfo&state=123#wechat_redirect";
        }

        public async Task<SelfHelpSettingDto> GetSelfHelpSettingAsync()
        {
            SelfHelpSettingDto selfHelpSettingDto = new SelfHelpSettingDto();
            return selfHelpSettingDto;
        }

        public async Task EditSelfHelpSettingAsync(SelfHelpSettingDto selfHelpSettingDto)
        {

        }
    }
}
