using Egoal.Extensions;

namespace Egoal.Payment.ABCPay
{
    public class WechatPayResponse : MessageBase
    {
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TrxType { get; set; }
        public string PaymentURL { get; set; }
        public string QRURL { get; set; }
        public string MerchantID { get; set; }
        public string OrderNo { get; set; }
        public string OrderAmount { get; set; }
        public string HostDate { get; set; }
        public string HostTime { get; set; }
        public string PrePayID { get; set; }
        public string ThirdOrderNo { get; set; }
        public string OpenID { get; set; }
        public string BankType { get; set; }
        public object APP { get; set; }
        public object JSAPI { get; set; }

        public NetPayResult ToNetPayResult()
        {
            var result = new NetPayResult();
            result.OpenId = OpenID;
            result.BankType = BankType;
            if (!OrderAmount.IsNullOrEmpty())
            {
                result.TotalFee = OrderAmount.To<decimal>();
            }
            result.TransactionId = ThirdOrderNo;
            result.ListNo = OrderNo;
            result.ErrorMessage = ErrorMessage;
            result.IsPaid = ReturnCode == "0000";
            result.IsPaying = ReturnCode == "AP6419";

            return result;
        }
    }
}
