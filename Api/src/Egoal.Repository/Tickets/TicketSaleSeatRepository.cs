using Dapper;
using Egoal.EntityFrameworkCore;
using Egoal.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Egoal.Extensions;

namespace Egoal.Tickets
{
    public class TicketSaleSeatRepository : EfCoreRepositoryBase<TicketSaleSeat, long>, ITicketSaleSeatRepository
    {
        public TicketSaleSeatRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public async Task<List<TicketSaleSeatDto>> GetTicketSeatsAsync(GetTicketSeatsInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), "d.OrderListNo=@ListNo");
            where.AppendWhereIf(input.TicketId.HasValue, "d.ID=@TicketId");

            string sql = $@"
SELECT
a.TradeID,
a.TicketID,
a.SeatID,
b.StadiumID,
b.RegionID,
a.SDate,
a.ChangCiID,
c.ID AS GroundId
FROM dbo.TM_TicketSaleSeat a WITH(NOLOCK)
LEFT JOIN dbo.SS_Seat b ON a.SeatID=b.ID
LEFT JOIN dbo.VM_Ground c ON b.StadiumID=c.StadiumID
LEFT JOIN dbo.TM_TicketSale d WITH(NOLOCK) ON a.TicketID=d.ID
{where}
";
            return (await Connection.QueryAsync<TicketSaleSeatDto>(sql, input, Transaction)).ToList();
        }

        public async Task<DataTable> StatGroundChangCiSaleAsync(StatGroundChangCiSaleInput input)
        {
            string sql = $@"
SELECT
a.GroundID,
b.ChangCiID,
c.STime,
c.ETime,
CASE WHEN MAX(c.ChangCiNum) IS NULL OR MAX(c.ChangCiNum)=0 THEN MAX(d.SeatNum) ELSE MAX(c.ChangCiNum) END AS TotalNum,
SUM(e.SaleNum) AS SaleNum,
0 AS SurplusNum
FROM dbo.TM_ChangCiPlan a WITH(NOLOCK)
JOIN dbo.TM_ChangCiGroupDetail b WITH(NOLOCK) ON b.ChangCiGroupID = a.ChangCiGroupID
JOIN dbo.TM_ChangCi c WITH(NOLOCK) ON c.ID=b.ChangCiID
JOIN dbo.VM_Ground d WITH(NOLOCK) ON d.ID=a.GroundID
LEFT JOIN dbo.TM_GroundDateChangCiSaleNum e WITH(NOLOCK) ON e.[Date]=a.[Date] AND e.GroundID=a.GroundID AND e.ChangCiID=c.ID
WHERE a.[Date]=@TravelDate AND d.ChangCiSaleFlag=1 AND (d.SeatSaleFlag=0 OR d.SeatSaleFlag IS NULL)
GROUP BY a.GroundID,b.ChangCiID,c.STime,c.ETime
UNION ALL
SELECT
a.GroundID,
b.ChangCiID,
c.STime,
c.ETime,
COUNT(DISTINCT e.ID) AS TotalNum,
COUNT(DISTINCT f.ID) AS SaleNum,
0 AS SurplusNum
FROM dbo.TM_ChangCiPlan a WITH(NOLOCK)
JOIN dbo.TM_ChangCiGroupDetail b WITH(NOLOCK) ON b.ChangCiGroupID = a.ChangCiGroupID
JOIN dbo.TM_ChangCi c WITH(NOLOCK) ON c.ID=b.ChangCiID
JOIN dbo.VM_Ground d WITH(NOLOCK) ON d.ID=a.GroundID
JOIN dbo.SS_Seat e WITH(NOLOCK) ON e.StadiumID=d.StadiumID
LEFT JOIN dbo.TM_TicketSaleSeat f WITH(NOLOCK) ON f.SDate=a.[Date] AND f.SeatID=e.ID AND f.ChangCiID=b.ChangCiID
WHERE a.[Date]=@TravelDate AND d.SeatSaleFlag=1 AND e.RegionID>0
GROUP BY a.GroundID,b.ChangCiID,c.STime,c.ETime
ORDER BY a.GroundID,c.STime
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}
