using System;

namespace Egoal.Payment
{
    public class QueryRefundResult
    {
        public string ListNo { get; set; }
        public string RefundListNo { get; set; }
        public decimal RefundFee { get; set; }
        public string RefundStatus { get; set; }
        public DateTime RefundTime { get; set; }
        public string RefundRecvAccount { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public bool ShouldRetry { get; set; }
        public bool IsExist { get; set; }
    }
}
