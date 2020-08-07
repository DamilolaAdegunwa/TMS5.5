using Egoal.Extensions;
using Egoal.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Egoal.Payment.SaobePay
{
    public class SaobePayApi
    {
        private readonly SaobePayOptions _options;
        private readonly ILogger _logger;
        private readonly HttpService _httpService;

        public SaobePayApi(
            IOptions<SaobePayOptions> options,
            ILogger<SaobePayApi> logger,
            HttpService httpService)
        {
            _options = options.Value;
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<JsApiPayResponse> JsApiPayAsync(JsApiPayRequest input)
        {
            input.service_id = "012";
            SetCommonValue(input);
            if (input.notify_url.IsNullOrEmpty())
            {
                input.notify_url = _options.WebApiUrl.UrlCombine("/Api/Payment/SaobeNotify");
            }

            input.MakeSign(_options.SaoBeAccessToken);

            var json = input.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/jspay");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var payResult = response.JsonToObject<JsApiPayResponse>();
            if (!payResult.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(payResult, originalData);

            return payResult;
        }

        public async Task<MicroPayResponse> MicroPayAsync(MicroPayRequest request)
        {
            request.service_id = "010";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/barcodepay");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<MicroPayResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<NativePayResponse> NativePayAsync(NativePayRequest input)
        {
            input.service_id = "011";
            SetCommonValue(input);
            if (input.notify_url.IsNullOrEmpty())
            {
                input.notify_url = _options.WebApiUrl.UrlCombine("/Api/Payment/SaobeNotify");
            }

            input.MakeSign(_options.SaoBeAccessToken);

            var json = input.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/prepay");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var payResult = response.JsonToObject<NativePayResponse>();
            if (!payResult.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(payResult, originalData);

            return payResult;
        }

        public NotifyRequest Notify(string data)
        {
            if (!data.IsJson())
            {
                throw new ApiException("通知数据格式错误", data);
            }

            var result = data.JsonToObject<NotifyRequest>();
            if (!result.CheckSign(_options.SaoBeAccessToken))
            {
                throw new ApiException("通知数据签名错误", data);
            }

            Log(result, data);

            return result;
        }

        public async Task<QueryOrderResponse> QueryPayAsync(QueryOrderRequest request)
        {
            request.service_id = "020";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/query");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<QueryOrderResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<CloseOrderResponse> ClosePayAsync(CloseOrderRequest request)
        {
            request.service_id = "041";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/close");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<CloseOrderResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<ReverseResponse> ReversePayAsync(ReverseRequest request)
        {
            request.service_id = "040";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/cancel");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<ReverseResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<RefundResponse> RefundAsync(RefundRequest request)
        {
            request.service_id = "030";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/refund");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<RefundResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<QueryRefundResponse> QueryRefundAsync(QueryRefundRequest request)
        {
            request.service_id = "031";
            SetCommonValue(request);

            request.MakeSign(_options.SaoBeAccessToken);

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/queryrefund");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<QueryRefundResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            Log(result, originalData);

            return result;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            request.pay_ver = "100";
            request.service_id = "090";
            request.merchant_no = _options.SaoBeMerchantNo;
            request.terminal_id = _options.SaoBeTerminalId;

            request.MakeSign();

            var json = request.ToJson();

            string url = _options.SaoBeDomainUrl.UrlCombine("/pay/100/sign");
            string response = await _httpService.PostJsonAsync(url, json);

            var originalData = $"{json}--{response}";

            if (!response.IsJson())
            {
                throw new ApiException("返回数据格式错误", originalData);
            }

            var result = response.JsonToObject<RegisterResponse>();
            if (!result.CheckSign())
            {
                throw new ApiException("返回数据签名错误", originalData);
            }

            var resultBase = new ResultBase
            {
                return_code = result.return_code,
                return_msg = result.return_msg,
                result_code = result.result_code
            };
            Log(resultBase, originalData);

            return result;
        }

        private void SetCommonValue(RequestBase input)
        {
            input.pay_ver = "100";
            input.merchant_no = _options.SaoBeMerchantNo;
            input.terminal_id = _options.SaoBeTerminalId;
        }

        private void Log(ResultBase result, string originalData)
        {
            if (result.return_code != "01")
            {
                _logger.LogError($"{result.return_msg}--{ originalData}");
            }
            else if (result.result_code != "01")
            {
                _logger.LogError($"{result.return_msg}--{ originalData}");
            }
            else
            {
                _logger.LogInformation(originalData);
            }
        }
    }
}
