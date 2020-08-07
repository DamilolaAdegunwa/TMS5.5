using System.Collections.Generic;

namespace Egoal.Payment.ABCPay
{
    public class RefundRequest
    {
        public RefundRequest()
        {
            SplitMerInfo = new Dictionary<int, SplitAccInfoItem>();
        }

        public string TrxType { get; } = "Refund";
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string MerRefundAccountNo { get; set; }
        public string MerRefundAccountName { get; set; }
        public string OrderNo { get; set; }
        public string NewOrderNo { get; set; }
        public string CurrencyCode { get; set; } = "156";
        public string TrxAmount { get; set; }
        public string RefundType { get; set; } = "0";
        public string MerchantRemarks { get; set; }
        public string MerRefundAccountFlag { get; set; }
        public Dictionary<int, SplitAccInfoItem> SplitMerInfo { get; set; }

        public class SplitAccInfoItem
        {
            public string SplitMerchantID { get; set; }
            public string SplitAmount { get; set; }
        }
    }
}
