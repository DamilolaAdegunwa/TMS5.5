using Egoal.Extensions;
using Egoal.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Egoal.Payment.Alipay
{
    public class AlipayApi
    {
        private readonly AlipayOptions _options;
        private readonly ILogger _logger;
        private readonly HttpService _httpService;

        public AlipayApi(
            IOptions<AlipayOptions> options,
            ILogger<AlipayApi> logger,
            HttpService httpService)
        {
            _options = options.Value;
            _logger = logger;
            _httpService = httpService;
        }

        public async Task<string> PageExecuteAsync(AlipayRequest alipayRequest)
        {
            alipayRequest.app_id = _options.AliAppID;
            alipayRequest.sign_type = _options.AliPaySignType;
            alipayRequest.timestamp = DateTime.Now.ToString(AlipayOptions.DateTimeFormat);

            var parameters = alipayRequest.ToJson().JsonToObject<Dictionary<string, string>>();
            parameters["sign"] = alipayRequest.sign = AlipaySignature.SignData(parameters, _options.AliPayMerChantPrivateKeyPath, alipayRequest.charset, _options.AliPaySignType);

            string html = BuildHtmlRequest(parameters, alipayRequest.charset, "POST", "POST");

            _logger.LogInformation(html);

            return await Task.FromResult(html);
        }

        private string BuildHtmlRequest(IDictionary<string, string> parameters, string charset, string method, string buttonValue)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append($"<form id='alipaysubmit' name='alipaysubmit' action='https://openapi.alipay.com/gateway.do?charset={charset}' method='{method}' style='display:none;'>");
            foreach (KeyValuePair<string, string> temp in parameters)
            {
                htmlBuilder.Append($"<input  name='{temp.Key}' value='{temp.Value}'/>");
            }
            htmlBuilder.Append($"<input type='submit' value='{buttonValue}'></form>");
            htmlBuilder.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return htmlBuilder.ToString();
        }

        public async Task<T> ExecuteAsync<T>(AlipayRequest alipayRequest) where T : AlipayResponse
        {
            string requestData = string.Empty;
            string responseData = string.Empty;

            try
            {
                alipayRequest.app_id = _options.AliAppID;
                alipayRequest.sign_type = _options.AliPaySignType;
                alipayRequest.timestamp = DateTime.Now.ToString(AlipayOptions.DateTimeFormat);

                var parameters = alipayRequest.ToJson().JsonToObject<Dictionary<string, string>>();
                parameters["sign"] = alipayRequest.sign = AlipaySignature.SignData(parameters, _options.AliPayMerChantPrivateKeyPath, alipayRequest.charset, _options.AliPaySignType);
                requestData = BuildQuery(parameters, alipayRequest.charset);

                string url = $"https://openapi.alipay.com/gateway.do?charset={alipayRequest.charset}";
                responseData = await _httpService.PostFormDataAsync(url, parameters, Encoding.GetEncoding(alipayRequest.charset));

                var responseBody = AlipaySignature.VerifyResponseData(responseData, alipayRequest.method, _options.AliPayPublicKeyPath, alipayRequest.charset, _options.AliPaySignType);

                T response = responseBody.JsonToObject<T>();

                Log(response, $"{requestData}--{responseData}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}--{requestData}--{responseData}");

                throw;
            }
        }

        private string BuildQuery(IDictionary<string, string> parameters, string charset)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var pair in parameters)
            {
                if (!pair.Key.IsNullOrEmpty() && !pair.Value.IsNullOrEmpty())
                {
                    string encodedValue = HttpUtility.UrlEncode(pair.Value, Encoding.GetEncoding(charset));

                    builder.Append(pair.Key).Append("=").Append(encodedValue).Append("&");
                }
            }

            return builder.ToString().TrimEnd('&');
        }

        private void Log(AlipayResponse response, string data)
        {
            if (response == null)
            {
                _logger.LogError($"数据解析失败--{data}");
            }
            else if (response.code != "10000")
            {
                _logger.LogError($"{response.msg}--{response.sub_msg}--{data}");
            }
            else
            {
                _logger.LogInformation(data);
            }
        }

        public NotifyRequest DeserializeNotify(string data)
        {
            var parameters = new SortedDictionary<string, string>();

            var pairs = data.Split('&');
            foreach (var pair in pairs)
            {
                var temp = pair.Split('=');
                var key = temp[0];
                var value = temp[1];

                if (!key.IsNullOrEmpty() && !value.IsNullOrEmpty())
                {
                    parameters.Add(key, value.UrlDecode());
                }
            }

            StringBuilder builder = new StringBuilder();
            foreach (var pair in parameters)
            {
                if (!pair.Key.Equals("sign", StringComparison.OrdinalIgnoreCase) && !pair.Key.Equals("sign_type", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
                }
            }

            var signContent = builder.ToString().TrimEnd('&');

            var request = parameters.ToJson().JsonToObject<NotifyRequest>();

            AlipaySignature.VerifyResponseSign(signContent, request.sign, _options.AliPayPublicKeyPath, "utf-8", request.sign_type);

            return request;
        }
    }
}
