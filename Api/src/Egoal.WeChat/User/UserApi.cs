using Egoal.Dependency;
using Egoal.Events.Bus;
using Egoal.Extensions;
using Egoal.Net.Http;
using Egoal.WeChat.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Egoal.WeChat.User
{
    public class UserApi : ApiBase, ITransientDependency
    {
        private readonly WeChatOptions _options;
        private readonly HttpService _httpService;
        private readonly OAuthApi _oAuthApi;

        public UserApi(
            ILogger<UserApi> logger,
            IEventBus eventBus,
            IOptions<WeChatOptions> options,
            HttpService httpService,
            OAuthApi oAuthApi)
            : base(logger, eventBus)
        {
            _options = options.Value;
            _httpService = httpService;
            _oAuthApi = oAuthApi;
        }

        public async Task<UserInfoResult> GetUserInfoAsync(string openId)
        {
            var accessToken = _options.AccessToken;
            var token = await _oAuthApi.GetAccessTokenAsync();
            accessToken = token.access_token;

            _options.AccessToken = accessToken;

            string url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken}&openid={openId}&lang=zh_CN";

            string response = await _httpService.GetStringAsync(url);

            var result = response.JsonToObject<UserInfoResult>();

            await EnsureSuccessAsync(result, $"{url}--{response}");

            return result;
        }
    }
}
