using Egoal.Payment.Dto;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public interface IPayAppService
    {
        Task PrePayAsync(PrePayInput payInput);
        Task<PayOutput> MicroPayAsync(MicroPayInput payInput);
        Task<string> OffiaccountPayAsync(string listNo);
        Task<string> NativePayAsync(NativePayInput input);
        Task<string> H5PayAsync(H5PayInput input);
        Task<string> MiniprogramPayAsync(string listNo);
        Task<PayOutput> CashPayAsync(string listNo);
        Task ClosePayAsync(string listNo);
        Task<NotifyResult> HandlePayNotifyAsync(string data, int payTypeId);
        Task LoopQueryNetPayAsync(QueryNetPayJobArgs input);
        Task ConfirmPayStatusAsync(ConfirmPayStatusJobArgs input);
        Task ApplyRefundAsync(RefundMoneyApply refundMoneyApply);
        Task RefundAsync(long id);
        Task<NetPayOrderDto> GetNetPayOrderAsync(string listNo);
    }
}
