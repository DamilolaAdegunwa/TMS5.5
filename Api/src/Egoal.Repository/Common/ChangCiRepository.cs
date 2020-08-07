using Dapper;
using Egoal.Common.Dto;
using Egoal.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egoal.Extensions;

namespace Egoal.Common
{
    public class ChangCiRepository : EfCoreRepositoryBase<ChangCi>, IChangCiRepository
    {
        public ChangCiRepository(IServiceProvider serviceProvider)
           : base(serviceProvider)
        { }

        public async Task<List<ChangCiPlanDto>> GetChangCiPlanAsync(string date, int? groundId = null, string stime = "")
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.[Date]=@date");
            where.AppendWhereIf(groundId.HasValue, "a.GroundID=@groundId");
            where.AppendWhereIf(!stime.IsNullOrEmpty(), "c.STime=@stime");

            string sql = $@"
SELECT
a.[Date],
a.GroundID,
c.ID AS ChangCiId,
c.[Name] AS ChangCiName,
c.STime,
c.ETime,
c.ChangCiNum,
c.ReservedNum
FROM dbo.TM_ChangCiPlan a
JOIN dbo.TM_ChangCiGroupDetail b ON b.ChangCiGroupID = a.ChangCiGroupID
JOIN dbo.TM_ChangCi c ON c.ID=b.ChangCiID
{where}
ORDER BY c.STime
";
            var items = await Connection.QueryAsync<ChangCiPlanDto>(sql, new { date, groundId, stime }, Transaction);

            return items.ToList();
        }
    }
}
