namespace Egoal.Payment.ABCPay
{
    public class PayCancelResponse
    {
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TrxType { get; set; }
        public string OrderInfo { get; set; }

        public ClosePayResult ToClosePayResult()
        {
            var result = new ClosePayResult();
            result.Success = ReturnCode == "0000" || ErrorMessage.Contains("订单不存在");

            return result;
        }
    }
}
