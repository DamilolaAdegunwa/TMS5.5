using Dapper;
using Egoal.EntityFrameworkCore;
using Egoal.Stadiums.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Stadiums
{
    public class SeatRepository : EfCoreRepositoryBase<Seat, long>, ISeatRepository
    {
        public SeatRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public async Task<List<SeatForSaleDto>> GetSeatForSaleAsync(SeatingInput input)
        {
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.AppendWhere("a.StadiumID=@StadiumId");
            whereBuilder.AppendWhere("a.RegionID>0");
            whereBuilder.AppendWhereIf(input.SeatTypeId.HasValue, "a.SeatTypeID=@SeatTypeId");
            whereBuilder.AppendWhere("a.ValidFlag=1");
            whereBuilder.AppendWhere("(b.ID IS NULL OR b.StatusID=1 OR (b.StatusID=3 AND b.LockTime<DATEADD(MINUTE,@LockMinutes,GETDATE())))");

            string sql = $@"
SELECT TOP(@Quantity)
a.ID,
b.ID AS StatusCacheId,
b.StatusID,
b.LockTime
FROM dbo.SS_Seat a WITH(NOLOCK)
LEFT JOIN
(
	SELECT
	ID,
	SeatID,
	StatusID,
	LockTime
	FROM dbo.TM_TicketSaleSeatStatusCache WITH(NOLOCK)
	WHERE SDate=@Date
	AND ChangCiID=@ChangCiId
)b ON a.ID=b.SeatID
{whereBuilder}
ORDER BY a.PrioritySaleFlag DESC,a.Code
";
            return (await Connection.QueryAsync<SeatForSaleDto>(sql, input, Transaction)).ToList();
        }
    }
}
