using Egoal.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Egoal.Payment.SaobePay
{
    public class PayService : INetPayService
    {
        private readonly SaobePayOptions _options;
        private readonly SaobePayApi _payApi;

        public PayService(
            IOptions<SaobePayOptions> options,
            SaobePayApi payApi)
        {
            _options = options.Value;
            _payApi = payApi;
        }

        public async Task<NetPayResult> MicroPayAsync(MicroPayCommand command)
        {
            var request = new MicroPayRequest();
            request.pay_type = "000";
            request.terminal_trace = command.ListNo;
            request.terminal_time = command.PayStartTime.ToString(SaobePayOptions.DateTimeFormat);
            request.auth_no = command.AuthCode;
            request.total_fee = (command.PayMoney * 100).ToString("F0");
            request.order_body = command.ProductInfo;
            request.attach = command.Attach;

            var result = await _payApi.MicroPayAsync(request);

            return result.ToPayOutput();
        }

        public async Task<string> JsApiPayAsync(JsApiPayCommand command)
        {
            var request = new JsApiPayRequest();
            request.pay_type = "010";
            request.terminal_trace = command.ListNo;
            request.terminal_time = command.PayStartTime.ToString(SaobePayOptions.DateTimeFormat);
            request.total_fee = (command.PayMoney * 100).ToString("F0");
            request.open_id = command.OpenId;
            request.order_body = command.ProductInfo;
            request.attach = command.Attach;

            var result = await _payApi.JsApiPayAsync(request);

            return new { result.appId, result.timeStamp, result.nonceStr, package = result.package_str, result.signType, result.paySign }.ToJson();
        }

        public async Task<string> NativePayAsync(NativePayCommand command)
        {
            var request = new NativePayRequest();
            request.pay_type = command.SubPayTypeId == 8 ? "010" : "020";
            request.terminal_trace = command.ListNo;
            request.terminal_time = command.PayStartTime.ToString(SaobePayOptions.DateTimeFormat);
            request.total_fee = (command.PayMoney * 100).ToString("F0");
            request.order_body = command.ProductInfo;
            request.attach = command.Attach;

            var result = await _payApi.NativePayAsync(request);

            return result.qr_code;
        }

        public Task<string> H5PayAsync(H5PayCommand command)
        {
            throw new NotImplementedException();
        }

        public NotifyCommand DeserializeNotify(string data)
        {
            var result = _payApi.Notify(data);

            return result.ToNotifyInput();
        }

        public NotifyResult BuildNotifyResult(bool success, string message = "")
        {
            var output = new NotifyResult();
            output.ContentType = "application/json";
            output.Data = new { return_code = success ? "01" : "02", return_msg = success ? "成功" : message }.ToJson();

            return output;
        }

        public async Task<QueryPayResult> QueryPayAsync(QueryPayCommand input)
        {
            var request = new QueryOrderRequest();
            request.pay_type = "000";
            request.terminal_trace = Guid.NewGuid().ToString().Replace("-", string.Empty);
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);
            request.pay_trace = input.ListNo;
            request.pay_time = input.PayTime.ToString(SaobePayOptions.DateTimeFormat);
            request.out_trade_no = input.TransactionId;

            var result = await _payApi.QueryPayAsync(request);

            return result.ToQueryPayOutput();
        }

        public async Task<ClosePayResult> ClosePayAsync(ClosePayCommand input)
        {
            var request = new CloseOrderRequest();
            request.pay_type = "010";
            request.terminal_trace = Guid.NewGuid().ToString().Replace("-", string.Empty);
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);
            request.pay_trace = input.ListNo;
            request.pay_time = input.PayTime.ToString(SaobePayOptions.DateTimeFormat);
            request.out_trade_no = input.TransactionId;

            var result = await _payApi.ClosePayAsync(request);

            return result.ToCloseOrderOutput();
        }

        public async Task<ReversePayResult> ReversePayAsync(ReversePayCommand input)
        {
            var request = new ReverseRequest();
            request.pay_type = "000";
            request.terminal_trace = Guid.NewGuid().ToString().Replace("-", string.Empty);
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);
            request.out_trade_no = input.TransactionId;
            request.pay_trace = input.ListNo;
            request.pay_time = input.PayTime.ToString(SaobePayOptions.DateTimeFormat);

            var result = await _payApi.ReversePayAsync(request);

            return result.ToReverseOutput();
        }

        public async Task<RefundResult> RefundAsync(RefundCommand input)
        {
            var request = new RefundRequest();
            request.pay_type = "000";
            request.terminal_trace = input.RefundListNo;
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);
            request.refund_fee = (input.RefundFee * 100).ToString("F0");
            request.out_trade_no = input.TransactionId;
            request.pay_trace = input.ListNo;
            request.pay_time = input.PayTime.ToString(SaobePayOptions.DateTimeFormat);

            var result = await _payApi.RefundAsync(request);

            return result.ToRefundOutput();
        }

        public async Task<QueryRefundResult> QueryRefundAsync(QueryRefundCommand input)
        {
            var request = new QueryRefundRequest();
            request.pay_type = "000";
            request.terminal_trace = Guid.NewGuid().ToString().Replace("-", string.Empty);
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);
            request.pay_trace = input.ListNo;
            request.pay_time = input.PayTime.ToString(SaobePayOptions.DateTimeFormat);
            request.out_refund_no = input.RefundListNo;

            var result = await _payApi.QueryRefundAsync(request);

            return result.ToQueryRefundOutput();
        }

        public async Task<string> RegisterAsync()
        {
            var request = new RegisterRequest();
            request.terminal_trace = Guid.NewGuid().ToString().Replace("-", string.Empty);
            request.terminal_time = DateTime.Now.ToString(SaobePayOptions.DateTimeFormat);

            var result = await _payApi.RegisterAsync(request);

            return result.access_token;
        }
    }
}
