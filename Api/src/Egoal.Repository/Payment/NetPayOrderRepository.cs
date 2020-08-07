using Dapper;
using Egoal.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public class NetPayOrderRepository : EfCoreRepositoryBase<NetPayOrder, long>, INetPayOrderRepository
    {
        public NetPayOrderRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<bool> SetPayTypeAsync(long id, int payTypeId, int subPayTypeId, OnlinePayTradeType onlinePayTradeType, NetPayType? netPayTypeId = null, string netPayTypeName = null)
        {
            string sql = @"
UPDATE dbo.OM_NetPayOrder SET
PayTypeId=@payTypeId,
SubPayTypeId=@subPayTypeId,
OnlinePayTradeType=@onlinePayTradeType,
NetPayTypeID=@netPayTypeId,
NetPayTypeName=@netPayTypeName
WHERE ID=@id
";
            var param = new { id, payTypeId, subPayTypeId, onlinePayTradeType, netPayTypeId, netPayTypeName };
            return (await Connection.ExecuteAsync(sql, param, Transaction)) > 0;
        }

        public async Task<bool> SetJsApiPayArgsAsync(long id, string jsApiPayArgs)
        {
            string sql = @"
UPDATE dbo.OM_NetPayOrder SET
JsApiPayArgs=@jsApiPayArgs
WHERE ID=@id
";
            return (await Connection.ExecuteAsync(sql, new { id, jsApiPayArgs }, Transaction)) > 0;
        }
    }
}
