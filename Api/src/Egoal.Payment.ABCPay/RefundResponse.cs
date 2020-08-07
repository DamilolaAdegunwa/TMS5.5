using Egoal.Extensions;

namespace Egoal.Payment.ABCPay
{
    public class RefundResponse
    {
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TrxType { get; set; }
        public string OrderNo { get; set; }
        public string NewOrderNo { get; set; }
        public string TrxAmount { get; set; }
        public string BatchNo { get; set; }
        public string VoucherNo { get; set; }
        public string HostDate { get; set; }
        public string HostTime { get; set; }
        public string iRspRef { get; set; }

        public RefundResult ToRefundResult()
        {
            var result = new RefundResult();
            result.ListNo = OrderNo;
            result.RefundId = VoucherNo;
            result.RefundFee = TrxAmount.To<decimal>();
            result.Success = ReturnCode == "0000";
            result.ShouldRetry = ReturnCode != "0000";
            result.ErrorMessage = ErrorMessage;

            return result;
        }
    }
}
