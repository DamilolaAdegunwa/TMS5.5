using System.Threading.Tasks;

namespace Egoal.Payment
{
    public interface INetPayService
    {
        Task<NetPayResult> MicroPayAsync(MicroPayCommand command);
        Task<string> JsApiPayAsync(JsApiPayCommand command);
        Task<string> NativePayAsync(NativePayCommand command);
        Task<string> H5PayAsync(H5PayCommand command);
        NotifyCommand DeserializeNotify(string data);
        NotifyResult BuildNotifyResult(bool success, string message = "");
        Task<QueryPayResult> QueryPayAsync(QueryPayCommand command);
        Task<ClosePayResult> ClosePayAsync(ClosePayCommand command);
        Task<ReversePayResult> ReversePayAsync(ReversePayCommand command);
        Task<RefundResult> RefundAsync(RefundCommand command);
        Task<QueryRefundResult> QueryRefundAsync(QueryRefundCommand command);
    }
}
