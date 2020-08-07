using Dapper;
using Egoal.Application.Services.Dto;
using Egoal.EntityFrameworkCore;
using Egoal.Extensions;
using Egoal.Tickets.Dto;
using Egoal.Trades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class TicketCheckRepository : EfCoreRepositoryBase<TicketCheck, long>, ITicketCheckRepository
    {
        public TicketCheckRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public async Task<PagedResultDto<TicketCheckListDto>> QueryTicketChecksAsync(QueryTicketCheckInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.StartCheckTime.HasValue, "a.CTime>=@StartCheckTime");
            where.AppendWhereIf(input.EndCheckTime.HasValue, "a.CTime<@EndCheckTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhereIf(input.TicketId.HasValue, "a.TicketID=@TicketId");
            where.AppendWhereIf(input.GroundId.HasValue, "a.GroundId=@GroundId");
            where.AppendWhereIf(input.GateGroupId.HasValue, "a.GateGroupId=@GateGroupId");
            where.AppendWhereIf(input.GateId.HasValue, "a.GateId=@GateId");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), "a.ListNo=@ListNo");
            where.AppendWhereIf(!input.TicketCode.IsNullOrEmpty(), "a.TicketCode=@TicketCode");
            where.AppendWhereIf(!input.CardNo.IsNullOrEmpty(), "a.CardNo=@CardNo");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcid.HasValue, "a.CashpcID=@CashPcid");
            where.AppendWhereIf(input.ParkId.HasValue, "a.ParkID=@ParkId");

            string sql = $@"
SELECT
a.ID,
a.CTime,
a.CardNo,
a.GroundName,
a.GateGroupName,
a.GateName,
a.ParkName,
a.TicketTypeName,
a.CheckTypeName,
a.TotalNum,
a.SurplusNum,
a.CheckNum,
a.RecycleFlagName,
a.TicketCode,
a.SaleParkName,
a.ListNo,
a.STime,
a.ETime,
a.CheckerName,
a.CashierName,
a.CashPCName,
a.SalePointName,
a.InOutFlagName,
a.UniqueID,
a.GlkOwnerName,
a.FxCardNo,
a.MemberName,
a.CustomerName,
a.GuiderName,
a.IsSecondInFlag,
ROW_NUMBER() OVER(ORDER BY a.CTime DESC) AS RowNum
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
{where}
";
            string pagedSql = $@"
SELECT
*
FROM
(
    {sql}
)x
WHERE x.RowNum BETWEEN @StartRowNum AND @EndRowNum
";
            string countSql = $@"
SELECT
COUNT(*)
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
{where}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var ticketChecks = await Connection.QueryAsync<TicketCheckListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<TicketCheckListDto>(count, ticketChecks.ToList());
            }
            else
            {
                var ticketChecks = await Connection.QueryAsync<TicketCheckListDto>(sql, input, Transaction);

                return new PagedResultDto<TicketCheckListDto>(ticketChecks.Count(), ticketChecks.ToList());
            }
        }

        /// <summary>
        /// 检票入园统计，按时间段
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> StatTicketCheckInByTimeAsync(StatTicketCheckInInput input)
        {
            string sql = @"
SELECT
*
FROM
(
	SELECT
	x.CDate AS 日期,
	x.CTP,
	SUM(x.CheckNum) AS CheckNum
	FROM
	(
		SELECT
		CASE WHEN CTP<'08' THEN '8点前' WHEN CTP>'19' THEN '20点后' ELSE CTP END AS CTP,
		CheckNum,
		CDate
		FROM dbo.TM_TicketCheck WITH(NOLOCK)
        {0}
	)x
	GROUP BY x.CDate,x.CTP
)y
PIVOT(SUM(y.CheckNum) FOR y.CTP IN ([8点前],{1}[20点后])) AS p
ORDER BY p.日期
";

            return await StatTicketCheckInByTimeAsync(input, sql);
        }

        public async Task<DataTable> StatTicketCheckInByTimeAsync(StatTicketCheckInInput input, string sql)
        {
            StringBuilder where = new StringBuilder();
            if (input.IfByGround)
            {
                input.StartDate = input.StartCTime.ToDateString();
                input.EndDate = input.EndCTime.ToDateString();
                where.AppendWhere("CDate>=@StartDate");
                where.AppendWhere("CDate<=@EndDate");
                where.AppendWhereIf(input.GroundId.HasValue, "GroundId = @GroundId");
            }
            else
            {
                where.AppendWhere("CTime>=@StartCTime");
                where.AppendWhere("CTime<@EndCTime");
            }
            where.AppendWhere("InOutFlag=1");

            StringBuilder columns = new StringBuilder();
            for (int i = 8; i <= 19; i++)
            {
                columns.Append("[").Append(i.ToString().PadLeft(2, '0')).Append("],");
            }

            sql = string.Format(sql, where, columns);

            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckDayInByTimeAsync(StatTicketCheckInInput input)
        {
            string sql = @"

SELECT
*
FROM
(
	SELECT
    b.Name AS 项目,
	x.CDate AS 日期,
	x.CTP,
	SUM(x.CheckNum) AS CheckNum
	FROM
	(
		SELECT
		CASE WHEN CTP<8 THEN '8点前' WHEN CTP = 8 THEN '08' WHEN CTP = 9 THEN '09' WHEN CTP>19 THEN '20点后' ELSE CTP END AS CTP,
		CheckNum,
		CDate,
		GroundId
		FROM dbo.TM_TicketCheckDayStat WITH(NOLOCK)
        {0}
	)x
	LEFT JOIN VM_Ground b on b.Id = x.GroundId
	GROUP BY b.Name,x.CDate,x.CTP
)y
PIVOT(SUM(y.CheckNum) FOR y.CTP IN ([8点前],{1}[20点后])) AS p
ORDER BY p.日期
";
            return await StatTicketCheckInByTimeAsync(input, sql);
        }

        /// <summary>
        /// 检票入园统计，多景点，按时间段
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> StatTicketCheckInByParkAndTimeAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");

            StringBuilder columns = new StringBuilder();
            for (int i = 8; i <= 19; i++)
            {
                columns.Append("[").Append(i.ToString().PadLeft(2, '0')).Append("],");
            }

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	CAST(x.ParkID AS VARCHAR(10)) AS 景点,
	x.CTP,
	SUM(x.CheckNum) AS CheckNum
	FROM
	(
		SELECT
		CASE WHEN a.CTP<'08' THEN '8点前' WHEN a.CTP>'19' THEN '20点后' ELSE a.CTP END AS CTP,
		a.CheckNum,
		b.ParkID
		FROM dbo.TM_TicketCheck a WITH(NOLOCK)
        LEFT JOIN dbo.VM_Ground b ON b.ID=a.GroundID
        {where}
	)x
	GROUP BY x.ParkID,x.CTP
)y
PIVOT(SUM(y.CheckNum) FOR y.CTP IN ([8点前],{columns}[20点后])) AS p
ORDER BY p.景点
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckByGroundAndTimeAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");

            StringBuilder columns = new StringBuilder();
            for (int i = 9; i <= 20; i++)
            {
                columns.Append("[").Append(i.ToString().PadLeft(2, '0')).Append("],");
            }

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	CAST(x.GroundID AS VARCHAR(10)) AS 检票区域,
	(CASE WHEN x.InOutFlag=1 THEN '入口' ELSE '出口' END) AS 出入类型,
	x.CTP,
	SUM(x.CheckNum) AS CheckNum
	FROM
	(
		SELECT
		CASE WHEN a.CTP<'09' THEN '9点前' WHEN a.CTP>'20' THEN '21点后' ELSE a.CTP END AS CTP,
		a.CheckNum,
		a.InOutFlag,
		a.GroundID
		FROM dbo.TM_TicketCheck a WITH(NOLOCK)
        {where}
	)x
	GROUP BY x.GroundID,x.InOutFlag,x.CTP
)y
PIVOT(SUM(y.CheckNum) FOR y.CTP IN ([9点前],{columns}[21点后])) AS p
ORDER BY p.检票区域
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        /// <summary>
        /// 检票入园统计
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> StatTicketCheckInAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            if (input.IfByGround)
            {
                input.StartDate = input.StartCTime.ToDateString();
                where.AppendWhere("CDate>=@StartDate");
                input.EndDate = input.EndCTime.ToDateString();
                where.AppendWhere("CDate<=@EndDate");
                where.AppendWhereIf(input.GroundId.HasValue, "GroundId = @GroundId");
            }
            else
            {
                where.AppendWhere("CTime>=@StartCTime");
                where.AppendWhere("CTime<@EndCTime");
            }
            where.AppendWhere("InOutFlag=1");

            string[] statTypes = new string[] { "CWeek", "CMonth", "CQuarter", "CYear" };
            string[] statTypeNames = new string[] { "星期", "月份", "季度", "年份" };
            string statType = statTypes[input.StatType - 2];
            string statTypeName = statTypeNames[input.StatType - 2];

            string sql = $@"
SELECT
{statType} AS {statTypeName},
SUM(CheckNum) AS 人数
FROM dbo.TM_TicketCheck WITH(NOLOCK)
{where}
GROUP BY {statType}
ORDER BY {statType}
";
            if (input.IfByGround)
            {
                sql = GetStatTicketCheckDayInSql(statType, statTypeName, where);
            }
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public string GetStatTicketCheckDayInSql(string statType, string statTypeName, StringBuilder where)
        {
            string statTypeSql = "";
            if (statType == "CMonth")
            {
                statTypeSql = "SUBSTRING(CDate,1,7)";
            }
            if (statType == "CYear")
            {
                statTypeSql = "SUBSTRING(CDate,1,4)";
            }

            string sql = $@"
select 
b.Name AS 项目,
{statTypeSql} AS {statTypeName},
SUM(CheckNum) AS 人数
 from dbo.TM_TicketCheckDayStat
 LEFT JOIN dbo.VM_Ground b on dbo.TM_TicketCheckDayStat.GroundId = b.Id
{where}
 Group By b.Name,{statTypeSql};
";
            return sql;
        }

        /// <summary>
        /// 检票入园统计，多景点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> StatTicketCheckInByParkAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");

            string[] statTypes = new string[] { "CWeek", "CMonth", "CQuarter", "CYear" };
            string statType = statTypes[input.StatType - 2];

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	CAST(b.ParkID AS VARCHAR(10)) AS 景点,
	a.{statType},
	SUM(a.CheckNum) AS CheckNum
	FROM dbo.TM_TicketCheck a WITH(NOLOCK)
    LEFT JOIN dbo.VM_Ground b ON b.ID=a.GroundID
	{where}
	GROUP BY b.ParkID,a.{statType}
)y
PIVOT(SUM(y.CheckNum) FOR y.{statType} IN ({SqlBuilder.BuildTimeStatTypeColumns(statType, input.StartCTime, input.EndCTime)})) AS p
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckInByDateAndParkAsync(StatTicketCheckInInput input, IEnumerable<ComboboxItemDto<int>> parks)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");

            StringBuilder parkColumns = new StringBuilder();
            foreach (var park in parks)
            {
                parkColumns.Append("[").Append(park.Value).Append("],");
            }

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	a.CDate AS 日期,
	b.ParkID,
	SUM(a.CheckNum) AS CheckNum
	FROM dbo.TM_TicketCheck a WITH(NOLOCK)
	LEFT JOIN dbo.VM_Ground b WITH(NOLOCK) ON a.GroundID=b.ID
    {where}
	GROUP BY a.CDate,b.ParkID
)x
PIVOT(SUM(x.CheckNum) FOR x.ParkID IN ({parkColumns.ToString().TrimEnd(',')})) AS p
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckInByGateGroupAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");
            where.AppendWhereIf(input.ParkId.HasValue, "b.ParkID=@ParkId");
            where.AppendWhereIf(input.GateGroupId.HasValue, "a.GateGroupID=@GateGroupId");

            string sql = $@"
SELECT
b.ParkID,
a.GateGroupID,
a.GateID,
SUM(a.CheckNum) AS CheckNum
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
LEFT JOIN dbo.VM_Ground b WITH(NOLOCK) ON a.GroundID=b.ID
{where}
GROUP BY b.ParkID,a.GateGroupID,a.GateID
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckInByGroundAndGateGroupAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");
            where.AppendWhereIf(input.GateGroupId.HasValue, "a.GateGroupID=@GateGroupId");

            string sql = $@"
SELECT
a.GroundID,
a.GateGroupID,
SUM(a.CheckNum) AS CheckNum
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
{where}
GROUP BY a.GroundID,a.GateGroupID
ORDER BY a.GroundID,a.GateGroupID
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckByTradeSourceAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");

            string[] statTypes = new string[] { "CDate", "CMonth", "CYear" };
            string[] statTypeNames = new string[] { "日期", "月份", "年份" };
            string statType = statTypes[input.StatType - 1];
            string statTypeName = statTypeNames[input.StatType - 1];

            var tradeSources = typeof(TradeSource).ToComboboxItems();
            StringBuilder tradeSourceColumns = new StringBuilder();
            StringBuilder tradeSourceNames = new StringBuilder();
            foreach (var tradeSource in tradeSources)
            {
                tradeSourceColumns.Append("[").Append(tradeSource.DisplayText).Append("],");
                tradeSourceNames.Append($"WHEN {tradeSource.Value} THEN '{tradeSource.DisplayText}' ");
            }

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	a.{statType} AS {statTypeName},
	CASE ISNULL(b.TradeSource,-1) WHEN -1 THEN '直接入园' {tradeSourceNames}END AS TradeSourceName,
	SUM(a.CheckNum) AS CheckNum
	FROM dbo.TM_TicketCheck a WITH(NOLOCK)
	LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
	{where}
	GROUP BY a.{statType},b.TradeSource
)x
PIVOT(SUM(x.CheckNum) FOR x.TradeSourceName IN ([直接入园],{tradeSourceColumns.ToString().TrimEnd(',')})) AS p
ORDER BY p.{statTypeName}
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTicketCheckInByTicketTypeAsync(StatTicketCheckInInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.InOutFlag=1");
            where.AppendWhereIf(input.GateId.HasValue, "a.GateID=@GateId");
            where.AppendWhereIf(input.CheckerId.HasValue, "a.CheckerID=@CheckerId");

            string sql = $@"
SELECT
TicketTypeID,
SUM(CheckNum) AS CheckNum
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
{where}
GROUP BY TicketTypeID
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatTouristNumAsync(StatTouristNumInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.SSDate.HasValue, "a.CDate>=@SSDate");
            where.AppendWhereIf(input.ESDate.HasValue, "a.CDate<=@ESDate");
            where.AppendWhereIf(input.GateGroupId.HasValue, "a.GateGroupId=@GateGroupId");
            where.AppendWhereIf(input.GateId.HasValue, "a.GateId=@GateId");
            StringBuilder inNumWhere = new StringBuilder();
            inNumWhere.Append(where);
            inNumWhere.AppendWhere("a.InOutFlag = '1'");
            StringBuilder outNumWhere = new StringBuilder();
            outNumWhere.Append(where);
            outNumWhere.AppendWhere("a.InOutFlag = '0'");

            string sql = $@"
select 
p.Name as ParkName
,g.Name as GroundName
,gt.Name as GateName
,a.InNum 
,b.OutNum
from
    (
     SELECT 
     a.GroundID
     ,a.GateID
     ,SUM(a.CheckNum) as InNum
     FROM TM_TicketCheckDayStat a
	 {inNumWhere}
    group by a.GroundID,a.GateID
    )a
LEFT JOIN 
	(
	SELECT
	 a.GroundID
	,a.GateID
	,SUM(a.CheckNum) AS OutNum
	FROM TM_TicketCheckDayStat a
	{outNumWhere}
	GROUP BY a.GroundID,a.GateID
	) b on b.GroundID = a.GroundID and b.GateID = a.GateID
LEFT JOIN VM_Ground g ON a.GroundID = g.ID
LEFT JOIN VM_Park p ON g.ParkID = p.ID
LEFT JOIN VM_Gate gt on a.GateID = gt.ID
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}
