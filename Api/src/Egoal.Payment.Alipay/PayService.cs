using Egoal.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Egoal.Payment.Alipay
{
    public class PayService : INetPayService
    {
        private readonly AlipayOptions _options;
        private readonly AlipayApi _alipayApi;
        private readonly string _notifyUrl;

        public PayService(
            IOptions<AlipayOptions> options,
            AlipayApi alipayApi)
        {
            _options = options.Value;
            _alipayApi = alipayApi;
            _notifyUrl = _options.WebApiUrl.UrlCombine("/Api/Payment/AlipayNotify");
        }

        public async Task<NetPayResult> MicroPayAsync(MicroPayCommand command)
        {
            PayRequest payRequest = new PayRequest();
            payRequest.out_trade_no = command.ListNo;
            payRequest.scene = "bar_code";
            payRequest.auth_code = command.AuthCode;
            payRequest.subject = command.ProductInfo;
            payRequest.body = command.Attach;
            payRequest.total_amount = command.PayMoney;
            payRequest.timeout_express = GetTimeoutExpress(command);

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.pay";
            alipayRequest.notify_url = _notifyUrl;
            alipayRequest.biz_content = payRequest.ToJson(false);

            PayResponse payResponse = await _alipayApi.ExecuteAsync<PayResponse>(alipayRequest);

            return payResponse.ToNetPayOutput();
        }

        public Task<string> JsApiPayAsync(JsApiPayCommand command)
        {
            throw new NotSupportedException("支付宝不支持微信JsApi支付");
        }

        public async Task<string> NativePayAsync(NativePayCommand command)
        {
            PrecreateRequest precreateRequest = new PrecreateRequest();
            precreateRequest.out_trade_no = command.ListNo;
            precreateRequest.total_amount = command.PayMoney;
            precreateRequest.subject = command.ProductInfo;
            precreateRequest.body = command.Attach;
            precreateRequest.timeout_express = GetTimeoutExpress(command);

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.precreate";
            alipayRequest.notify_url = _notifyUrl;
            alipayRequest.biz_content = precreateRequest.ToJson(false);

            PrecreateResponse precreateResponse = await _alipayApi.ExecuteAsync<PrecreateResponse>(alipayRequest);

            return precreateResponse.qr_code;
        }

        public async Task<string> H5PayAsync(H5PayCommand command)
        {
            WapPayRequest wapPayRequest = new WapPayRequest();
            wapPayRequest.out_trade_no = command.ListNo;
            wapPayRequest.total_amount = command.PayMoney;
            wapPayRequest.subject = command.ProductInfo;
            wapPayRequest.body = command.Attach;
            wapPayRequest.product_code = "QUICK_WAP_WAY";
            wapPayRequest.timeout_express = GetTimeoutExpress(command);

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.wap.pay";
            alipayRequest.return_url = command.WapUrl;
            alipayRequest.notify_url = _notifyUrl;
            alipayRequest.biz_content = wapPayRequest.ToJson(false);

            return await _alipayApi.PageExecuteAsync(alipayRequest);
        }

        private string GetTimeoutExpress(PayCommandBase command)
        {
            var minutes = Math.Ceiling((command.PayExpireTime - DateTime.Now).TotalMinutes);

            return $"{Math.Max(minutes, 1).To<int>()}m";
        }

        public NotifyCommand DeserializeNotify(string data)
        {
            var request = _alipayApi.DeserializeNotify(data);

            return request.ToNotifyInput();
        }

        public NotifyResult BuildNotifyResult(bool success, string message = "")
        {
            return new NotifyResult { Data = success ? "success" : message };
        }

        public async Task<QueryPayResult> QueryPayAsync(QueryPayCommand input)
        {
            QueryRequest queryRequest = new QueryRequest();
            queryRequest.out_trade_no = input.ListNo;
            queryRequest.trade_no = input.TransactionId;

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.query";
            alipayRequest.biz_content = queryRequest.ToJson(false);

            QueryResponse queryResponse = await _alipayApi.ExecuteAsync<QueryResponse>(alipayRequest);

            return queryResponse.ToQueryPayOutput();
        }

        public async Task<ClosePayResult> ClosePayAsync(ClosePayCommand input)
        {
            CloseRequest closeRequest = new CloseRequest();
            closeRequest.out_trade_no = input.ListNo;
            closeRequest.trade_no = input.TransactionId;

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.close";
            alipayRequest.biz_content = closeRequest.ToJson(false);

            CloseResponse closeResponse = await _alipayApi.ExecuteAsync<CloseResponse>(alipayRequest);

            return closeResponse.ToClosePayOutput();
        }

        public async Task<ReversePayResult> ReversePayAsync(ReversePayCommand input)
        {
            CancelRequest cancelRequest = new CancelRequest();
            cancelRequest.out_trade_no = input.ListNo;
            cancelRequest.trade_no = input.TransactionId;

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.cancel";
            alipayRequest.biz_content = cancelRequest.ToJson(false);

            CancelResponse cancelResponse = await _alipayApi.ExecuteAsync<CancelResponse>(alipayRequest);

            return cancelResponse.ToReversePayOutput();
        }

        public async Task<RefundResult> RefundAsync(RefundCommand input)
        {
            RefundRequest refundRequest = new RefundRequest();
            refundRequest.out_trade_no = input.ListNo;
            refundRequest.trade_no = input.TransactionId;
            refundRequest.out_request_no = input.RefundListNo;
            refundRequest.refund_amount = input.RefundFee;

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.refund";
            alipayRequest.biz_content = refundRequest.ToJson(false);

            RefundResponse refundResponse = await _alipayApi.ExecuteAsync<RefundResponse>(alipayRequest);

            var output = refundResponse.ToRefundOutput();
            if (output.RefundId.IsNullOrEmpty())
            {
                output.RefundId = input.RefundListNo;
            }

            return output;
        }

        public async Task<QueryRefundResult> QueryRefundAsync(QueryRefundCommand input)
        {
            QueryRefundRequest queryRefundRequest = new QueryRefundRequest();
            queryRefundRequest.out_trade_no = input.ListNo;
            queryRefundRequest.trade_no = input.TransactionId;
            queryRefundRequest.out_request_no = input.RefundListNo;

            AlipayRequest alipayRequest = new AlipayRequest();
            alipayRequest.method = "alipay.trade.fastpay.refund.query";
            alipayRequest.biz_content = queryRefundRequest.ToJson(false);

            QueryRefundResponse queryRefundResponse = await _alipayApi.ExecuteAsync<QueryRefundResponse>(alipayRequest);

            return queryRefundResponse.ToQueryRefundOutput();
        }
    }
}
