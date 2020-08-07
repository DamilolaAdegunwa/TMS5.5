using Dapper;
using Egoal.Application.Services.Dto;
using Egoal.EntityFrameworkCore;
using Egoal.Extensions;
using Egoal.ValueCards.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.ValueCards
{
    public class CzkDetailRepository : EfCoreRepositoryBase<CzkDetail, long>, ICzkDetailRepository
    {
        public CzkDetailRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public async Task<PagedResultDto<CzkDetailListDto>> QueryCzkDetailsAsync(QueryCzkDetailInput input)
        {
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.AppendWhere("a.CTime>=@StartCTime");
            whereBuilder.AppendWhere("a.CTime<@EndCTime");
            whereBuilder.AppendWhere("a.CommitFlag=1");
            whereBuilder.AppendWhereIf(input.CzkOpTypeId.HasValue, "a.CzkOpTypeID=@CzkOpTypeId");
            whereBuilder.AppendWhereIf(input.CzkConsumeTypeId.HasValue, "a.CzkConsumeTypeId=@CzkConsumeTypeId");
            whereBuilder.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeId=@TicketTypeId");
            whereBuilder.AppendWhereIf(input.CashierId.HasValue, "a.CashierId=@CashierId");
            whereBuilder.AppendWhereIf(input.MemberId.HasValue, "a.MemberId=@MemberId");
            whereBuilder.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), "a.ListNo=@ListNo");
            whereBuilder.AppendWhereIf(!input.CardNo.IsNullOrEmpty(), "a.CardNo=@CardNo");

            StringBuilder rechargeWhereBuilder = new StringBuilder(" ");
            rechargeWhereBuilder.AppendWhereIf(input.CzkRechargeTypeId.HasValue, "a.CzkRechargeTypeId=@CzkRechargeTypeId");
            rechargeWhereBuilder.AppendWhereIf(input.CzkCztcId.HasValue, "a.CzkCztcId=@CzkCztcId");
            rechargeWhereBuilder.AppendWhereIf(input.PayTypeId.HasValue, "b.PayTypeID=@PayTypeId");

            StringBuilder consumeWhereBuilder = new StringBuilder(" ");
            consumeWhereBuilder.AppendWhereIf(input.CzkRechargeTypeId.HasValue || input.CzkCztcId.HasValue || input.PayTypeId.HasValue, "1<>1");

            string sql = $@"
SELECT
x.*,
ROW_NUMBER() OVER(ORDER BY x.CTime DESC) AS RowNum
FROM
(
	SELECT
	a.CzkOpTypeID,
	a.ListNo,
	a.TicketCode,
	a.CardNo,
	a.TicketTypeID,
	a.CzkRechargeTypeID,
	a.CzkConsumeTypeID,
	a.CzkCztcID,
	a.OldCardMoney,
	a.OldFreeMoney,
	a.OldGameMoney,
	a.OldTotalMoney,
	a.RechargeCardMoney,
	a.RechargeFreeMoney,
	a.RechargeGameMoney,
	a.RechargeTotalMoney,
	a.UseCouponNum,
	a.ConsumeCardMoney,
	a.ConsumeFreeMoney,
	a.ConsumeGameMoney,
	a.ConsumeTotalMoney,
	a.NewCardMoney,
	a.NewFreeMoney,
	a.NewGameMoney,
	a.NewTotalMoney,
	a.YaJin,
	a.MemberID,
	b.PayTypeID,
	a.CashierID,
	NULL AS GroundID,
	a.Memo,
	a.CTime
	FROM dbo.MM_CzkDetail a WITH(NOLOCK)
	LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
    {whereBuilder}
    {rechargeWhereBuilder}
	UNION ALL
	SELECT
	a.CzkOpTypeID,
	a.ListNo,
	a.TicketCode,
	a.CardNo,
	a.TicketTypeID,
	NULL AS CzkRechargeTypeID,
	a.CzkConsumeTypeID,
	NULL AS CzkCztcID,
	a.OldCardMoney,
	a.OldFreeMoney,
	a.OldGameMoney,
	a.OldTotalMoney,
	NULL AS RechargeCardMoney,
	NULL AS RechargeFreeMoney,
	NULL AS RechargeGameMoney,
	NULL AS RechargeTotalMoney,
	NULL AS UseCouponNum,
	a.ConsumeCardMoney,
	a.ConsumeFreeMoney,
	a.ConsumeGameMoney,
	a.ConsumeTotalMoney,
	a.NewCardMoney,
	a.NewFreeMoney,
	a.NewGameMoney,
	a.NewTotalMoney,
	NULL AS YaJin,
	a.MemberID,
	NULL AS PayTypeID,
	NULL AS CashierID,
	b.GroundID,
	a.Memo,
	a.CTime
	FROM dbo.MM_MemberConsume a WITH(NOLOCK)
	LEFT JOIN dbo.TM_TicketCheck b WITH(NOLOCK) ON b.SyncCode=a.TicketCheckSyncCode
    {whereBuilder}
    {consumeWhereBuilder}
)x
";
            string pagedSql = $@"
SELECT
*
FROM
(
	{sql}
)y
WHERE y.RowNum BETWEEN @StartRowNum AND @EndRowNum
";
            string countSql = $@"
SELECT
SUM(x.TotalCount) AS TotalCount
FROM
(
	SELECT
	COUNT(*) AS TotalCount
	FROM dbo.MM_CzkDetail a WITH(NOLOCK)
	LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
    {whereBuilder}
    {rechargeWhereBuilder}
	UNION ALL
	SELECT
	COUNT(*) AS TotalCount
	FROM dbo.MM_MemberConsume a WITH(NOLOCK)
	LEFT JOIN dbo.TM_TicketCheck b WITH(NOLOCK) ON b.SyncCode=a.TicketCheckSyncCode
    {whereBuilder}
    {consumeWhereBuilder}
)x
";
            int count = 0;
            IEnumerable<CzkDetailListDto> items = null;
            if (input.ShouldPage)
            {
                count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                items = count > 0 ? await Connection.QueryAsync<CzkDetailListDto>(pagedSql, input, Transaction) : new List<CzkDetailListDto>();
            }
            else
            {
                items = await Connection.QueryAsync<CzkDetailListDto>(sql, input, Transaction);
                count = items.Count();
            }

            return new PagedResultDto<CzkDetailListDto>(count, items.ToList());
        }
    }
}
