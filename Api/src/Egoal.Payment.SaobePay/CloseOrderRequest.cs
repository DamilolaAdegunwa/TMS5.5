﻿namespace Egoal.Payment.SaobePay
{
    public class CloseOrderRequest : RequestBase
    {
        public string out_trade_no { get; set; }
        public string pay_trace { get; set; }
        public string pay_time { get; set; }
    }
}
