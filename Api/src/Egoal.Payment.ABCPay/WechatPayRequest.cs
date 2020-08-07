using System.Collections.Generic;

namespace Egoal.Payment.ABCPay
{
    public class WechatPayRequest
    {
        public WechatPayRequest()
        {
            Order = new OrderInfo();
            OrderItems = new Dictionary<int, OrderItem>();
            SplitAccInfoItems = new Dictionary<int, SplitAccInfoItem>();
            H5SceneInfo = new H5Scene();
        }

        public string TrxType { get; } = "UnifiedOrderReq";
        public string PaymentType { get; set; } = "8";
        public string PaymentLinkType { get; set; } = "1";
        public string NotifyType { get; set; } = "1";
        public string ResultNotifyURL { get; set; }
        public string MerchantRemarks { get; set; }
        public string IsBreakAccount { get; set; } = "0";
        public string CommodityType { get; set; } = "0202";
        public OrderInfo Order { get; set; }
        public Dictionary<int, OrderItem> OrderItems { get; set; }
        public Dictionary<int, SplitAccInfoItem> SplitAccInfoItems { get; set; }
        public H5Scene H5SceneInfo { get; set; }

        public class OrderInfo
        {
            public string PayTypeID { get; set; }
            public string OrderDate { get; set; }
            public string OrderTime { get; set; }
            public string ExpiredDate { get; set; }
            public string CurrencyCode { get; set; } = "156";
            public string OrderNo { get; set; }
            public string OrderAmount { get; set; }
            public string Fee { get; set; }
            public string AccountNo { get; set; }
            public string OpenID { get; set; }
            public string ReceiverAddress { get; set; }
            public string InstallmentMark { get; set; } = "0";
            public string InstallmentCode { get; set; }
            public string InstallmentNum { get; set; }
            public string BuyIP { get; set; }
            public string OrderDesc { get; set; }
            public string orderTimeoutDate { get; set; }
            public string LimitPay { get; set; }
        }

        public class OrderItem
        {
            public string SubMerName { get; set; }
            public string SubMerId { get; set; }
            public string SubMerMCC { get; set; }
            public string SubMerchantRemarks { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string UnitPrice { get; set; }
            public string Qty { get; set; }
            public string ProductRemarks { get; set; }
            public string ProductType { get; set; }
            public string ProductDiscount { get; set; }
            public string ProductExpiredDate { get; set; }
        }

        public class SplitAccInfoItem
        {
            public string SplitMerchantID { get; set; }
            public string SplitAmount { get; set; }
        }

        public class H5Scene
        {
            public string H5SceneType { get; set; }
            public string H5SceneUrl { get; set; }
            public string H5SceneName { get; set; }
        }
    }
}
