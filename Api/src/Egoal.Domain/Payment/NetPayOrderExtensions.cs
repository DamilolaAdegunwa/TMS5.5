using Egoal.Extensions;
using Egoal.UI;

namespace Egoal.Payment
{
    public static class NetPayOrderExtensions
    {
        public static NetPayType GetJsApiNetPayType(this NetPayOrder netPayOrder, int payTypeId)
        {
            switch (payTypeId)
            {
                case DefaultPayType.微信支付:
                    {
                        return NetPayType.WeiXinJsApiPay;
                    }
                case DefaultPayType.威富通支付:
                    {
                        return NetPayType.WFTJsApiPay;
                    }
                case DefaultPayType.扫呗支付:
                    {
                        return NetPayType.SaoBeJsApiPay;
                    }
                case DefaultPayType.农行支付:
                    {
                        return NetPayType.ABCJsApiPay;
                    }
                default:
                    {
                        throw new UserFriendlyException($"付款方式{payTypeId}不支持微信公众号支付");
                    }
            }
        }

        public static NetPayType GetMicroNetPayType(this NetPayOrder netPayOrder, int payTypeId)
        {
            switch (payTypeId)
            {
                case DefaultPayType.微信支付:
                    {
                        return NetPayType.WeiXinScanCardPay;
                    }
                case DefaultPayType.支付宝付款:
                    {
                        return NetPayType.AliBarcodePay;
                    }
                case DefaultPayType.威富通支付:
                    {
                        return NetPayType.WFTScanCardPay;
                    }
                case DefaultPayType.扫呗支付:
                    {
                        return NetPayType.SaoBeMicroPay;
                    }
                case DefaultPayType.农行支付:
                    {
                        return NetPayType.ABCMicroPay;
                    }
                default:
                    {
                        throw new UserFriendlyException($"付款方式{payTypeId}不支持刷卡支付");
                    }
            }
        }

        public static bool IsMicroPay(this NetPayOrder netPayOrder)
        {
            if (netPayOrder.OnlinePayTradeType.HasValue)
            {
                return netPayOrder.OnlinePayTradeType == OnlinePayTradeType.MICROPAY;
            }

            if (!netPayOrder.NetPayTypeId.HasValue) return false;

            return netPayOrder.NetPayTypeId.IsIn(NetPayType.WeiXinScanCardPay, NetPayType.AliBarcodePay, NetPayType.WFTScanCardPay, NetPayType.SaoBeMicroPay, NetPayType.ABCMicroPay);
        }

        public static NetPayType GetNativeNetPayType(this NetPayOrder netPayOrder, int payTypeId)
        {
            switch (payTypeId)
            {
                case DefaultPayType.微信支付:
                    {
                        return NetPayType.WeiXinScanQRCodePay;
                    }
                case DefaultPayType.支付宝付款:
                    {
                        return NetPayType.AliScanQRCodePay;
                    }
                case DefaultPayType.威富通支付:
                    {
                        return NetPayType.WFTScanCardPayDLL;
                    }
                case DefaultPayType.扫呗支付:
                    {
                        return NetPayType.SaoBeNativePay;
                    }
                case DefaultPayType.农行支付:
                    {
                        return NetPayType.ABCNativePay;
                    }
                default:
                    {
                        throw new UserFriendlyException($"付款方式{payTypeId}不支持扫码支付");
                    }
            }
        }
    }
}
