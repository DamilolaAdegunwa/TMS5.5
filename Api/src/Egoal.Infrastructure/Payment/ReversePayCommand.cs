using System;

namespace Egoal.Payment
{
    public class ReversePayCommand
    {
        public string ListNo { get; set; }
        public string TransactionId { get; set; }
        public OnlinePayTradeType OnlinePayTradeType { get; set; }
        public string SubPayTypeId { get; set; }
        public DateTime PayTime { get; set; }
    }
}
