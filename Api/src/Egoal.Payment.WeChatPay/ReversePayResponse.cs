namespace Egoal.Payment.WeChatPay
{
    public class ReversePayResponse : ResultBase
    {
        /// <summary>
        /// 是否需要继续调用撤销
        /// Y-需要，N-不需要
        /// </summary>
        public string recall { get; set; }

        public ReversePayResult ToReversePayOutput()
        {
            var output = new ReversePayResult();
            output.Success = result_code == "SUCCESS";
            output.ShouldRetry = recall == "Y" || err_code == "USERPAYING" || err_code == "SYSTEMERROR";
            output.ErrorMessage = return_code == "SUCCESS" ? err_code_des : return_msg;

            return output;
        }
    }
}
