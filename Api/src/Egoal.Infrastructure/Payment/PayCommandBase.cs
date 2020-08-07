using System;

namespace Egoal.Payment
{
    public class PayCommandBase
    {
        public int SubPayTypeId { get; set; }
        public OnlinePayTradeType OnlinePayTradeType { get; set; }
        public string ListNo { get; set; }
        public decimal PayMoney { get; set; }
        public string ProductInfo { get; set; }
        public string ProductId { get; set; }
        public string ClientIp { get; set; }
        public string Attach { get; set; }
        public DateTime PayStartTime { get; set; }
        public DateTime PayExpireTime { get; set; }
    }
}
