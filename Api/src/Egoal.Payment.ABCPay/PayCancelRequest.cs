namespace Egoal.Payment.ABCPay
{
    public class PayCancelRequest
    {
        public string TrxType { get; } = "MicroPayCancel";
        public string OrderNo { get; set; }
        public string QueryDetail { get; set; } = "false";
        public string SubMchNO { get; set; }
        public string ModelFlag { get; set; }
        public string MerchantFlag { get; set; }
    }
}
