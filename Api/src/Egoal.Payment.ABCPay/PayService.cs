using Egoal.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Egoal.Payment.ABCPay
{
    public class PayService : INetPayService
    {
        private readonly ABCPayOptions _options;
        private readonly ABCPayApi _payApi;
        private readonly string _notifyUrl;

        public PayService(
            IOptions<ABCPayOptions> options,
            ABCPayApi payApi)
        {
            _options = options.Value;
            _payApi = payApi;
            _notifyUrl = _options.WebApiUrl.UrlCombine("/Api/Payment/ABCPayNotify");
        }

        public async Task<NetPayResult> MicroPayAsync(MicroPayCommand command)
        {
            if (command.SubPayTypeId == 8)
            {
                return await WechatMicroPayAsync(command);
            }
            else
            {
                return await AliMicroPayAsync(command);
            }
        }

        private async Task<NetPayResult> WechatMicroPayAsync(MicroPayCommand command)
        {
            WechatPayRequest request = new WechatPayRequest();
            request.ResultNotifyURL = _notifyUrl;

            request.Order.PayTypeID = "MICROPAY";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.AccountNo = command.AuthCode;
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString("yyyyMMddHHmmss");

            request.OrderItems.Add(1, new WechatPayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<WechatPayResponse>(request);

            return response.ToNetPayResult();
        }

        private async Task<NetPayResult> AliMicroPayAsync(MicroPayCommand command)
        {
            AlipayRequest request = new AlipayRequest();
            request.ResultNotifyURL = _notifyUrl;
            request.TimeoutExpress = GetTimeoutExpress(command);

            request.Order.PayTypeID = "ALI_PAY";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.AccountNo = command.AuthCode;
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString("yyyyMMddHHmmss");

            request.OrderItems.Add(1, new AlipayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<AlipayResponse>(request);

            return response.ToNetPayResult();
        }

        public async Task<string> JsApiPayAsync(JsApiPayCommand command)
        {
            WechatPayRequest request = new WechatPayRequest();
            request.ResultNotifyURL = _notifyUrl;

            request.Order.PayTypeID = "JSAPI";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.AccountNo = command.OnlinePayTradeType == OnlinePayTradeType.MINIPROGRAM ? _options.WxMiniprogramAppID : _options.WxAppID;
            request.Order.OpenID = command.OpenId;
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString(ABCPayOptions.DateTimeFormat);

            request.OrderItems.Add(1, new WechatPayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<WechatPayResponse>(request);
            if (response.ReturnCode == "0000")
            {
                return response.JSAPI.ToJson().Replace("sub_appId", "appId");
            }

            return string.Empty;
        }

        public async Task<string> NativePayAsync(NativePayCommand command)
        {
            if (command.SubPayTypeId == 8)
            {
                return await WechatNativePayAsync(command);
            }
            else
            {
                return await AliNativePayAsync(command);
            }
        }

        private async Task<string> WechatNativePayAsync(NativePayCommand command)
        {
            WechatPayRequest request = new WechatPayRequest();
            request.ResultNotifyURL = _notifyUrl;

            request.Order.PayTypeID = "NATIVE";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString(ABCPayOptions.DateTimeFormat);

            request.OrderItems.Add(1, new WechatPayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<WechatPayResponse>(request);
            if (response.ReturnCode == "0000")
            {
                return response.QRURL;
            }

            return string.Empty;
        }

        private async Task<string> AliNativePayAsync(NativePayCommand command)
        {
            AlipayRequest request = new AlipayRequest();
            request.ResultNotifyURL = _notifyUrl;
            request.TimeoutExpress = GetTimeoutExpress(command);

            request.Order.PayTypeID = "ALI_PRECREATE";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString("yyyyMMddHHmmss");

            request.OrderItems.Add(1, new AlipayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<AlipayResponse>(request);

            return response.QRURL;
        }

        public async Task<string> H5PayAsync(H5PayCommand command)
        {
            if (command.SubPayTypeId == 8)
            {
                return await WechatH5PayAsync(command);
            }
            else
            {
                return await AliH5PayAsync(command);
            }
        }

        private async Task<string> WechatH5PayAsync(H5PayCommand command)
        {
            WechatPayRequest request = new WechatPayRequest();
            request.ResultNotifyURL = _notifyUrl;

            request.Order.PayTypeID = "MWEB";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString(ABCPayOptions.DateTimeFormat);

            request.OrderItems.Add(1, new WechatPayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            request.H5SceneInfo.H5SceneType = "Wap";
            request.H5SceneInfo.H5SceneName = command.WapName;
            request.H5SceneInfo.H5SceneUrl = command.WapUrl;

            var response = await _payApi.ExecuteAsync<WechatPayResponse>(request);
            if (response.ReturnCode == "0000")
            {
                return response.PaymentURL;
            }

            return string.Empty;
        }

        private async Task<string> AliH5PayAsync(H5PayCommand command)
        {
            AlipayRequest request = new AlipayRequest();
            request.ResultNotifyURL = _notifyUrl;
            request.WapQuitUrl = command.WapUrl;
            request.TimeoutExpress = GetTimeoutExpress(command);

            request.Order.PayTypeID = "ALI_WAP";
            request.Order.OrderDate = command.PayStartTime.ToString("yyyy-MM-dd").Replace('-', '/');
            request.Order.OrderTime = command.PayStartTime.ToString("HH:mm:ss");
            request.Order.OrderNo = command.ListNo;
            request.Order.OrderAmount = command.PayMoney.ToString();
            request.Order.BuyIP = command.ClientIp;
            request.Order.OrderDesc = command.ProductInfo;
            request.Order.orderTimeoutDate = command.PayExpireTime.ToString("yyyyMMddHHmmss");

            request.OrderItems.Add(1, new AlipayRequest.OrderItem
            {
                ProductID = command.ProductId,
                ProductName = command.ProductInfo
            });

            var response = await _payApi.ExecuteAsync<AlipayResponse>(request);

            return response.QRURL;
        }

        private string GetTimeoutExpress(PayCommandBase command)
        {
            var minutes = Math.Ceiling((command.PayExpireTime - DateTime.Now).TotalMinutes);

            return $"{Math.Max(minutes, 1).To<int>()}m";
        }

        public NotifyCommand DeserializeNotify(string data)
        {
            var request = _payApi.DeserializeNotify(data);

            return request.ToNotifyCommand();
        }

        public NotifyResult BuildNotifyResult(bool success, string message = "")
        {
            return new NotifyResult { Data = $"<URL>{_notifyUrl}</URL>" };
        }

        public async Task<QueryPayResult> QueryPayAsync(QueryPayCommand command)
        {
            var request = new QueryOrderRequest();
            request.PayTypeID = "ImmediatePay";
            request.OrderNo = command.ListNo;
            request.QueryDetail = "true";

            var response = await _payApi.ExecuteAsync<ResponseMessage<QueryOrderResponse>>(request);

            return response.TrxResponse.ToQueryPayResult();
        }

        public async Task<ClosePayResult> ClosePayAsync(ClosePayCommand command)
        {
            var request = new PayCancelRequest();
            request.OrderNo = command.ListNo;
            request.ModelFlag = "0";
            request.MerchantFlag = command.SubPayTypeId == "8" ? "W" : "Z";

            var response = await _payApi.ExecuteAsync<ResponseMessage<PayCancelResponse>>(request);

            return response.TrxResponse.ToClosePayResult();
        }

        public async Task<ReversePayResult> ReversePayAsync(ReversePayCommand command)
        {
            var queryCommand = new QueryPayCommand();
            queryCommand.ListNo = command.ListNo;
            queryCommand.TransactionId = command.TransactionId;
            queryCommand.OnlinePayTradeType = command.OnlinePayTradeType;
            queryCommand.SubPayTypeId = command.SubPayTypeId;
            queryCommand.PayTime = command.PayTime;

            var queryResult = await QueryPayAsync(queryCommand);
            if (queryResult.IsPaid)
            {
                var refundCommand = new RefundCommand();
                refundCommand.ListNo = command.ListNo;
                refundCommand.RefundListNo = DateTime.Now.Ticks.ToString();
                refundCommand.TransactionId = command.TransactionId;
                refundCommand.OnlinePayTradeType = command.OnlinePayTradeType;
                refundCommand.SubPayTypeId = command.SubPayTypeId;
                refundCommand.TotalFee = queryResult.TotalFee;
                refundCommand.RefundFee = queryResult.TotalFee;
                refundCommand.PayTime = command.PayTime;

                var refundResult = await RefundAsync(refundCommand);

                return new ReversePayResult
                {
                    Success = refundResult.Success,
                    ShouldRetry = refundResult.ShouldRetry,
                    ErrorMessage = refundResult.ErrorMessage
                };
            }
            else
            {
                var closeCommand = new ClosePayCommand();
                closeCommand.ListNo = command.ListNo;
                closeCommand.TransactionId = command.TransactionId;
                closeCommand.SubPayTypeId = command.SubPayTypeId;
                closeCommand.OnlinePayTradeType = command.OnlinePayTradeType;
                closeCommand.PayTime = command.PayTime;

                var closeResult = await ClosePayAsync(closeCommand);

                return new ReversePayResult
                {
                    Success = closeResult.Success,
                    ShouldRetry = false
                };
            }
        }

        public async Task<RefundResult> RefundAsync(RefundCommand command)
        {
            var request = new RefundRequest();
            request.OrderDate = DateTime.Now.ToString("yyyy-MM-dd").Replace('-', '/');
            request.OrderTime = DateTime.Now.ToString("HH:mm:ss");
            request.OrderNo = command.ListNo;
            request.NewOrderNo = command.RefundListNo;
            request.TrxAmount = command.RefundFee.ToString();

            var response = await _payApi.ExecuteAsync<ResponseMessage<RefundResponse>>(request);

            return response.TrxResponse.ToRefundResult();
        }

        public async Task<QueryRefundResult> QueryRefundAsync(QueryRefundCommand command)
        {
            var request = new QueryOrderRequest();
            request.PayTypeID = "Refund";
            request.OrderNo = command.ListNo;
            request.QueryDetail = "true";

            var response = await _payApi.ExecuteAsync<ResponseMessage<QueryOrderResponse>>(request);

            return response.TrxResponse.ToQueryRefundResult();
        }
    }
}
