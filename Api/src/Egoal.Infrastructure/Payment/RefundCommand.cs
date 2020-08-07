using System;

namespace Egoal.Payment
{
    public class RefundCommand
    {
        public string ListNo { get; set; }
        public string RefundListNo { get; set; }
        public string TransactionId { get; set; }
        public OnlinePayTradeType OnlinePayTradeType { get; set; }
        public string SubPayTypeId { get; set; }
        public decimal TotalFee { get; set; }
        public decimal RefundFee { get; set; }
        public DateTime PayTime { get; set; }
    }
}
