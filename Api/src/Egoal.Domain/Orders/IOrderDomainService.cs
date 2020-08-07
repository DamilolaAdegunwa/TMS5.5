using System.Threading.Tasks;

namespace Egoal.Orders
{
    public interface IOrderDomainService
    {
        Task CreateAsync(Order order);
        Task<bool> AllowCancelAsync(string listNo);
        Task CancelAsync(Order order);
        Task ConsumeAsync(string listNo);
        Task ConsumeAsync(string listNo, long orderDetailId, int consumeNum);
    }
}
