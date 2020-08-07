namespace Egoal.Payment.WeChatPay
{
    public class ClosePayResponse : ResultBase
    {
        /// <summary>
        /// 业务结果描述
        /// </summary>
        public string result_msg { get; set; }

        public ClosePayResult ToClosePayOutput()
        {
            var output = new ClosePayResult();
            output.Success = result_code?.ToUpper() == "SUCCESS" || err_code?.ToUpper() == "ORDERCLOSED";
            output.IsPaid = err_code?.ToUpper() == "ORDERPAID";

            return output;
        }
    }
}
