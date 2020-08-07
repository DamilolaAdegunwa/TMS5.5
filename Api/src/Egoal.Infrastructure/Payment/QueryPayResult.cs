namespace Egoal.Payment
{
    public class QueryPayResult : NotifyCommand
    {
        public string ErrorMessage { get; set; }
        public string TradeState { get; set; }
        public bool IsPaid { get; set; }
        public bool IsPaying { get; set; }
        public bool IsExist { get; set; }
        public bool IsRefund { get; set; }
    }
}
