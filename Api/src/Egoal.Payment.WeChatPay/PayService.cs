using Egoal.Extensions;
using Egoal.WeChat;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Egoal.Payment.WeChatPay
{
    public class PayService : INetPayService
    {
        private readonly WeChatOptions _options;
        private readonly WeChatPayApi _wxPayApi;

        public PayService(
            IOptions<WeChatOptions> options,
            WeChatPayApi payApi)
        {
            _options = options.Value;
            _wxPayApi = payApi;
        }

        public async Task<NetPayResult> MicroPayAsync(MicroPayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("auth_code", command.AuthCode);
            data.SetValue("body", command.ProductInfo);
            data.SetValue("total_fee", (command.PayMoney * 100).ToString("F0"));
            data.SetValue("out_trade_no", command.ListNo);
            data.SetValue("spbill_create_ip", command.ClientIp);
            SetAttach(data, command);
            SetPayTime(data, command);

            var result = await _wxPayApi.MicroPayAsync(data);

            return result.ToPayOutput();
        }

        public async Task<string> JsApiPayAsync(JsApiPayCommand commond)
        {
            var appId = GetAppId(commond.OnlinePayTradeType);

            PayData data = new PayData();
            data.SetValue("appid", appId);
            data.SetValue("body", commond.ProductInfo);
            data.SetValue("out_trade_no", commond.ListNo);
            data.SetValue("total_fee", (commond.PayMoney * 100).ToString("F0"));
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", commond.OpenId);
            data.SetValue("spbill_create_ip", commond.ClientIp);
            SetAttach(data, commond);
            SetPayTime(data, commond);

            var result = await _wxPayApi.UnifiedOrderAsync(data);

            PayData jsApiParam = new PayData();
            jsApiParam.SetValue("appId", appId);
            jsApiParam.SetValue("timeStamp", _wxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", _wxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", $"prepay_id={result.prepay_id}");
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign(_options.WxApiKey));

            string parameters = jsApiParam.ToJson();

            return parameters;
        }

        public async Task<string> NativePayAsync(NativePayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("body", command.ProductInfo);
            data.SetValue("out_trade_no", command.ListNo);
            data.SetValue("total_fee", (command.PayMoney * 100).ToString("F0"));
            data.SetValue("trade_type", "NATIVE");
            data.SetValue("product_id", command.ProductId);
            data.SetValue("spbill_create_ip", command.ClientIp);
            SetAttach(data, command);
            SetPayTime(data, command);

            var result = await _wxPayApi.UnifiedOrderAsync(data);

            return result.code_url;
        }

        public async Task<string> H5PayAsync(H5PayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("body", command.ProductInfo);
            data.SetValue("out_trade_no", command.ListNo);
            data.SetValue("total_fee", (command.PayMoney * 100).ToString("F0"));
            data.SetValue("trade_type", "MWEB");
            data.SetValue("spbill_create_ip", command.ClientIp);
            SetAttach(data, command);
            SetPayTime(data, command);

            var h5Info = new
            {
                h5_info = new H5Info
                {
                    type = "Wap",
                    wap_url = command.WapUrl,
                    wap_name = command.WapName
                }
            };
            data.SetValue("scene_info", h5Info.ToJson());

            var result = await _wxPayApi.UnifiedOrderAsync(data);

            return result.mweb_url;
        }

        private void SetAttach(PayData data, PayCommandBase command)
        {
            if (command.Attach.IsNullOrEmpty()) return;

            data.SetValue("attach", command.Attach);
        }

        private void SetPayTime(PayData data, PayCommandBase command)
        {
            var now = DateTime.Now;
            var minExpireTime = now.AddMinutes(_options.MinExpireMinutes);
            if (command.PayExpireTime < minExpireTime)
            {
                data.SetValue("time_start", now.ToString(WeChatOptions.DateTimeFormat));
                data.SetValue("time_expire", minExpireTime.ToString(WeChatOptions.DateTimeFormat));
            }
            else
            {
                data.SetValue("time_start", command.PayStartTime.ToString(WeChatOptions.DateTimeFormat));
                data.SetValue("time_expire", command.PayExpireTime.ToString(WeChatOptions.DateTimeFormat));
            }
        }

        public NotifyCommand DeserializeNotify(string xml)
        {
            var result = _wxPayApi.DeserializeNotify(xml);

            return result.ToNotifyInput();
        }

        public NotifyResult BuildNotifyResult(bool success, string message = "")
        {
            var data = new PayData();
            data.SetValue("return_code", success ? "SUCCESS" : "FAIL");
            data.SetValue("return_msg", success ? "OK" : message);

            var output = new NotifyResult();
            output.ContentType = "text/xml";
            output.Data = data.ToXml();

            return output;
        }

        public async Task<QueryPayResult> QueryPayAsync(QueryPayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            if (!command.ListNo.IsNullOrEmpty())
            {
                data.SetValue("out_trade_no", command.ListNo);
            }
            if (!command.TransactionId.IsNullOrEmpty())
            {
                data.SetValue("transaction_id", command.TransactionId);
            }

            var result = await _wxPayApi.QueryPayAsync(data);

            return result.ToQueryPayOutput();
        }

        public async Task<ClosePayResult> ClosePayAsync(ClosePayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("out_trade_no", command.ListNo);

            var result = await _wxPayApi.ClosePayAsync(data);
            return result.ToClosePayOutput();
        }

        public async Task<ReversePayResult> ReversePayAsync(ReversePayCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            if (!command.ListNo.IsNullOrEmpty())
            {
                data.SetValue("out_trade_no", command.ListNo);
            }
            if (!command.TransactionId.IsNullOrEmpty())
            {
                data.SetValue("transaction_id", command.TransactionId);
            }

            var result = await _wxPayApi.ReversePayAsync(data);

            return result.ToReversePayOutput();
        }

        public async Task<RefundResult> RefundAsync(RefundCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("out_trade_no", command.ListNo);
            data.SetValue("out_refund_no", command.RefundListNo);
            data.SetValue("total_fee", (command.TotalFee * 100).ToString("F0"));
            data.SetValue("refund_fee", (command.RefundFee * 100).ToString("F0"));

            var result = await _wxPayApi.RefundAsync(data);

            return result.ToRefundOutput();
        }

        public async Task<QueryRefundResult> QueryRefundAsync(QueryRefundCommand command)
        {
            PayData data = new PayData();
            data.SetValue("appid", GetAppId(command.OnlinePayTradeType));
            data.SetValue("out_trade_no", command.ListNo);
            data.SetValue("out_refund_no", command.RefundListNo);

            var result = await _wxPayApi.QueryRefundAsync(data);

            return result.ToQueryRefundOutput();
        }

        private string GetAppId(OnlinePayTradeType tradeType)
        {
            return tradeType == OnlinePayTradeType.MINIPROGRAM ? _options.WxMiniprogramAppID : _options.WxAppID;
        }
    }
}
