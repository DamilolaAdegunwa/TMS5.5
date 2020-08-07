namespace Egoal.Payment.ABCPay
{
    public class QueryOrderRequest
    {
        public string TrxType { get; } = "Query";
        public string PayTypeID { get; set; }
        public string OrderNo { get; set; }
        public string QueryDetail { get; set; }
    }
}
