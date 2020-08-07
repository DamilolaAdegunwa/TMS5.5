using Egoal.Domain.Repositories;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public interface INetPayOrderRepository : IRepository<NetPayOrder, long>
    {
        Task<bool> SetPayTypeAsync(long id, int payTypeId, int subPayTypeId, OnlinePayTradeType onlinePayTradeType, NetPayType? netPayTypeId = null, string netPayTypeName = null);
        Task<bool> SetJsApiPayArgsAsync(long id, string jsApiPayArgs);
    }
}
