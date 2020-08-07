using Dapper;
using Egoal.Application.Services.Dto;
using Egoal.EntityFrameworkCore;
using Egoal.Extensions;
using Egoal.Scenics;
using Egoal.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class TicketSaleRepository : EfCoreRepositoryBase<TicketSale, long>, ITicketSaleRepository
    {
        public TicketSaleRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public async Task InValidAsync(TicketSale ticketSale)
        {
            string sql = @"
DELETE dbo.TM_TicketGroundCache WHERE TicketID=@TicketID
DELETE dbo.TM_TicketGround WHERE TicketID=@TicketID
";
            await Connection.ExecuteAsync(sql, new { TicketID = ticketSale.Id }, Transaction, TimeoutSeconds);
        }

        public async Task RefundAsync(TicketSale ticketSale, int quantity)
        {
            string sql = @"
UPDATE dbo.TM_TicketGroundCache SET SurplusNum=SurplusNum-@quantity WHERE TicketID=@TicketID
UPDATE dbo.TM_TicketGround SET SurplusNum=SurplusNum-@quantity WHERE TicketID=@TicketID
";
            await Connection.ExecuteAsync(sql, new { TicketID = ticketSale.Id, quantity }, Transaction, TimeoutSeconds);
        }

        public async Task<int> GetFingerprintQuantityAsync(long ticketId)
        {
            string sql = @"
SELECT
COUNT(*)
FROM dbo.TM_TicketSaleFinger WITH(NOLOCK)
WHERE TicketID=@ticketId
";
            return await Connection.ExecuteScalarAsync<int>(sql, new { ticketId }, Transaction, TimeoutSeconds);
        }

        public async Task<DateTime> GetFacePhotoBindTimeAsync(long ticketId)
        {
            string sql = @"
SELECT TOP 1
CTime
FROM dbo.TM_TicketSalePhoto WITH(NOLOCK)
WHERE TicketID=@ticketId
AND PhotoTemplate IS NOT NULL
";
            return await Connection.ExecuteScalarAsync<DateTime>(sql, new { ticketId }, Transaction, TimeoutSeconds);
        }

        public async Task<PagedResultDto<TicketSale>> GetTicketsByMemberAsync(GetTicketsByMemberInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.TicketStatusID<=9");
            where.AppendWhere("a.SurplusNum>0");
            where.AppendWhere("a.ETime>=@ETime");
            where.AppendWhere("b.TradeSource=@TradeSource");
            if (input.CustomerId.HasValue)
            {
                where.AppendWhere("a.MemberID=@MemberId OR a.CustomerID=@CustomerId");
            }
            else
            {
                where.AppendWhere("a.MemberID=@MemberId AND a.CustomerID IS NULL");
            }

            string sql = $@"
SELECT
a.*,
ROW_NUMBER() OVER(ORDER BY a.CTime DESC) AS RowNum
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
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
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
{where}
";
            int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction, TimeoutSeconds);
            var tickets = await Connection.QueryAsync<TicketSale>(pagedSql, input, Transaction, TimeoutSeconds);

            return new PagedResultDto<TicketSale>(count, tickets.ToList());
        }

        public async Task<PagedResultDto<TicketSaleListDto>> QueryTicketSalesAsync(QueryTicketSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.StartSaleTime.HasValue, "a.CTime>=@StartSaleTime");
            where.AppendWhereIf(input.EndSaleTime.HasValue, "a.CTime<@EndSaleTime");
            where.AppendWhereIf(!input.StartTravelDate.IsNullOrEmpty(), "a.SDate>=@StartTravelDate");
            where.AppendWhereIf(!input.EndTravelDate.IsNullOrEmpty(), "a.SDate<=@EndTravelDate");
            where.AppendWhereIf(!input.TicketCode.IsNullOrEmpty(), "a.TicketCode=@TicketCode");
            where.AppendWhereIf(!input.CardNo.IsNullOrEmpty(), "a.CardNo=@CardNo");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), "a.ListNo=@ListNo");
            where.AppendWhereIf(input.TicketStatusId.HasValue, "a.TicketStatusID=@TicketStatusId");
            where.AppendWhereIf(input.IsExpired.HasValue, $"a.ETime{(input.IsExpired == true ? "<" : ">=")}@Now");
            where.AppendWhereIf(input.TicketTypeTypeId.HasValue, "a.TicketTypeTypeID=@TicketTypeTypeId");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.CustomerId.HasValue, "a.CustomerID=@CustomerId");
            where.AppendWhereIf(input.MemberId.HasValue, "a.MemberID=@MemberId");
            where.AppendWhereIf(input.PromoterId.HasValue, "a.PromoterId=@PromoterId");
            where.AppendWhereIf(input.ParkId.HasValue, "a.ParkID=@ParkId");
            where.AppendWhereIf(input.SalePointId.HasValue, "a.SalePointID=@SalePointId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashpcId.HasValue, "a.CashpcID=@CashpcId");
            where.AppendWhereIf(!input.OrderListNo.IsNullOrEmpty(), "a.OrderListNo=@OrderListNo");
            where.AppendWhereIf(!input.ThirdListNo.IsNullOrEmpty(), "b.ThirdPartyPlatformOrderID=@ThirdListNo");
            where.AppendWhereIf(input.TradeSource.HasValue, "b.TradeSource=@TradeSource");
            where.AppendWhereIf(input.PayTypeId.HasValue, "b.PayTypeID=@PayTypeId");
            where.AppendWhereIf(input.SalesManId.HasValue, "a.SalesManID=@SalesManId");
            where.AppendWhereIf(!input.CertNo.IsNullOrEmpty(), "(b.ContactCertNo=@CertNo OR EXISTS(SELECT TOP 1 1 FROM dbo.TM_TicketSaleBuyer WITH(NOLOCK) WHERE TicketID=a.ID AND CertNo=@CertNo))");
            where.AppendWhereIf(!input.Mobile.IsNullOrEmpty(), "(b.Mobile=@Mobile OR EXISTS(SELECT TOP 1 1 FROM dbo.TM_TicketSaleBuyer WITH(NOLOCK) WHERE TicketID=a.ID AND Mobile=@Mobile))");
            where.AppendWhereIf(input.HasFaceImage.HasValue, "a.PhotoBindFlag=@HasFaceImage");
            if (input.HasFingerprint.HasValue)
            {
                where.AppendWhere($"a.FingerStatusID={(input.HasFingerprint.Value ? "2" : "1")}");
            }
            where.AppendWhere("a.CommitFlag=1");

            string sql = $@"
SELECT
a.ID AS Id,
a.ListNo,
a.TicketCode,
a.CardNo,
a.FingerStatusID,
a.PhotoBindFlag,
a.ValidFlag,
a.TicketStatusID,
a.TicketStatusName,
a.TicketTypeID,
a.TicketTypeName,
a.DiscountTypeName,
a.DiscountRate,
a.TicPrice,
a.ReaPrice AS RealPrice,
a.TicMoney,
a.ReaMoney AS RealMoney,
a.PayTypeName,
a.PersonNum,
a.TotalNum,
a.STime,
a.ETime,
a.CashierId,
a.CashierName,
a.CashPCName,
a.SalePointName,
a.SalesmanName,
a.MemberID,
a.MemberName,
a.CustomerID,
a.CustomerName,
a.GuiderID,
a.GuiderName,
a.PromoterId,
a.ReturnTypeName,
a.ReturnRate,
a.OrderListNo,
(CASE WHEN a.CertTypeName IS NOT NULL THEN a.CertTypeName ELSE c.CertTypeName END) AS CertTypeName,
(CASE WHEN a.CertNo IS NOT NULL THEN a.CertNo ELSE c.CertNo END) AS CertNo,
a.CTime,
a.ParkName,
a.Memo,
b.TradeSource,
b.ThirdPartyPlatformOrderID,
b.ContactName,
b.Mobile AS ContactMobile,
b.ContactCertNo,
ROW_NUMBER() OVER(ORDER BY a.CTime DESC) AS RowNum
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
LEFT JOIN dbo.TM_TicketSaleBuyer c ON a.Id = c.TicketId
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
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
LEFT JOIN dbo.TM_TicketSaleBuyer c ON a.Id = c.TicketId
{where}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction, TimeoutSeconds);
                var tickets = await Connection.QueryAsync<TicketSaleListDto>(pagedSql, input, Transaction, TimeoutSeconds);

                return new PagedResultDto<TicketSaleListDto>(count, tickets.ToList());
            }
            else
            {
                var tickets = await Connection.QueryAsync<TicketSaleListDto>(sql, input, Transaction, TimeoutSeconds);

                return new PagedResultDto<TicketSaleListDto>(tickets.Count(), tickets.ToList());
            }
        }

        public async Task<List<StatTicketSaleListDto>> StatAsync(StatTicketSaleInput input)
        {
            var statColumns = new[] { "a.CDate", "a.CWeek", "a.CMonth", "a.CQuarter", "a.CYear", "a.TicketTypeID", "b.TradeSource" };
            var statColumn = statColumns[(int)input.StatType - 1];

            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.StartCTime.HasValue, "a.CTime>=@StartCTime");
            where.AppendWhereIf(input.EndCTime.HasValue, "a.CTime<@EndCTime");
            where.AppendWhereIf(!input.StartTravelDate.IsNullOrEmpty(), "a.SDate>=@StartTravelDate");
            where.AppendWhereIf(!input.EndTravelDate.IsNullOrEmpty(), "a.SDate<=@EndTravelDate");
            where.AppendWhereIf(input.TicketTypeTypeId.HasValue, "a.TicketTypeTypeID=@TicketTypeTypeId");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.TicketStatusId.HasValue, "a.TicketStatusID=@TicketStatusId");
            where.AppendWhereIf(input.SalePointId.HasValue, "a.SalePointID=@SalePointId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashpcId.HasValue, "a.CashpcID=@CashpcId");
            where.AppendWhereIf(input.TradeSource.HasValue, "b.TradeSource=@TradeSource");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag=1");

            string sql = $@"
SELECT
x.*,
x.SaleNum-x.ReturnNum AS RealNum,
x.SalePersonNum-x.ReturnPersonNum AS RealPersonNum,
x.SaleMoney-x.ReturnMoney AS RealMoney
FROM
(
    SELECT
    {statColumn} AS StatType,
    SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.TicketNum END) AS SaleNum,
    SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.PersonNum END) AS SalePersonNum,
    SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.ReaMoney END) AS SaleMoney,
    SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.TicketNum) ELSE 0 END) AS ReturnNum,
    SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.PersonNum) ELSE 0 END) AS ReturnPersonNum,
    SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.ReaMoney) ELSE 0 END) AS ReturnMoney
    FROM dbo.TM_TicketSale a WITH(NOLOCK)
    LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID
    {where}
    GROUP BY {statColumn}
)x
ORDER BY x.StatType
";
            return (await Connection.QueryAsync<StatTicketSaleListDto>(sql, input, Transaction, TimeoutSeconds)).ToList();
        }

        /// <summary>
        /// 收银员销售汇总
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> StatCashierSaleAsync(StatCashierSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.SalePointId.HasValue, "a.SalePointID=@SalePointId");

            string sql = $@"
SELECT
CashierID,
TicketTypeName,
1 AS TicketTypeTypeID,
ReaPrice AS RealPrice,
SUM(CASE WHEN TicketStatusID=11 THEN 0 ELSE PersonNum END) AS SaleNum,
SUM(CASE WHEN TicketStatusID=11 THEN 0 ELSE ABS(ReaMoney)+ABS(ISNULL(YaJin,0)) END) AS SaleMoney,
SUM(CASE WHEN TicketStatusID=11 THEN ABS(PersonNum) ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN TicketStatusID=11 THEN ABS(ReaMoney)+ABS(ISNULL(YaJin,0)) ELSE 0 END) AS ReturnMoney,
SUM(CASE WHEN TicketStatusID=11 THEN -ABS(ReaMoney)-ABS(ISNULL(YaJin,0)) ELSE ABS(ReaMoney)+ABS(ISNULL(YaJin,0)) END) AS RealMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
{where}
AND CommitFlag=1
AND StatFlag=1
GROUP BY CashierID,TicketTypeName,ReaPrice
UNION ALL
SELECT
CashierID,
TicketTypeName + '售卡' AS TicketTypeName,
8 AS TicketTypeTypeID,
NULL AS RealPrice,
SUM(CASE WHEN CzkRechargeTypeID = 1 THEN 1 ELSE 0 END) AS SaleNum,
SUM(CASE WHEN CzkRechargeTypeID = 1 THEN RealPrice+YaJin ELSE 0 END) AS SaleMoney,
SUM(CASE WHEN CzkRechargeTypeID IN (3,6) THEN 1 ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN CzkRechargeTypeID IN (3,6) THEN ABS(RechargeCardMoney)+ABS(YaJin) ELSE 0 END) AS ReturnMoney,
SUM(CASE WHEN CzkRechargeTypeID = 1 THEN RealPrice+YaJin ELSE 0 END)-SUM(CASE WHEN CzkRechargeTypeID IN (3,6) THEN ABS(RechargeCardMoney)+ABS(YaJin) ELSE 0 END) AS RealMoney
FROM dbo.MM_CzkDetail a WITH(NOLOCK)
{where}
AND CommitFlag=1
AND StatFlag=1
AND CzkRechargeTypeID IN (1,3,6)
GROUP BY CashierID,TicketTypeName
UNION ALL
SELECT
CashierID,
'充值' AS TicketTypeName,
9 AS TicketTypeTypeID,
NULL AS RealPrice,
SUM(1) AS SaleNum,
SUM(RechargeCardMoney) AS SaleMoney,
NULL AS ReturnNum,
NULL AS ReturnMoney,
SUM(RechargeCardMoney) AS RealMoney
FROM dbo.MM_CzkDetail a WITH(NOLOCK)
{where}
AND CommitFlag=1
AND StatFlag=1
AND CzkRechargeTypeID=2
GROUP BY CashierID
";
            if (input.StatTypeId == 2)
            {
                sql = $@"
SELECT
CashierID,
TicketTypeName,
1 AS TicketTypeTypeID,
ReaPrice AS RealPrice,
PayTypeId,
PayTypeName,
SUM(CASE WHEN TicketStatusId=11 THEN -ABS(ISNULL(PersonNum,0)) ELSE ABS(ISNULL(PersonNum,0)) END) PayTypeRealNum,
SUM(CASE WHEN TicketStatusID=11 THEN -ABS(ISNULL(ReaMoney,0))-ABS(ISNULL(YaJin,0)) ELSE ABS(ISNULL(ReaMoney,0))+ABS(ISNULL(YaJin,0)) END) AS PayTypeRealMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
{where}
AND CommitFlag=1
AND StatFlag=1
GROUP BY CashierID,TicketTypeName,ReaPrice,PayTypeId,PayTypeName

UNION ALL
SELECT
a.CashierID,
TicketTypeName + '售卡' AS TicketTypeName,
8 AS TicketTypeTypeID,
NULL AS RealPrice,
b.PayTypeId,
b.PayTypeName,
SUM(CASE WHEN CzkRechargeTypeId IN (3,6) THEN -1 ELSE 1 END) AS PayTypeNum,
SUM(CASE WHEN CzkRechargeTypeID = 1 THEN ISNULL(RealPrice+a.YaJin,0) ELSE 0 END)-SUM(CASE WHEN CzkRechargeTypeID IN (3,6) THEN ABS(ISNULL(RechargeCardMoney,0))+ABS(ISNULL(a.YaJin,0)) ELSE 0 END) AS PayTypeRealMoney
FROM dbo.MM_CzkDetail a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b ON a.TradeID = b.Id
{where}
AND a.CommitFlag=1
AND a.StatFlag=1
AND CzkRechargeTypeID IN (1,3,6)
GROUP BY a.CashierID,TicketTypeName,PayTypeId,PayTypeName

UNION ALL
SELECT
a.CashierID,
'充值' AS TicketTypeName,
9 AS TicketTypeTypeID,
NULL AS RealPrice,
b.PayTypeId,
b.PayTypeName,
SUM(1) AS PayTypeNum,
SUM(ISNULL(RechargeCardMoney,0)) AS PayTypeRealMoney
FROM dbo.MM_CzkDetail a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b ON a.TradeId = b.Id
{where}
AND a.CommitFlag=1
AND a.StatFlag=1
AND CzkRechargeTypeID=2
GROUP BY a.CashierID,b.PayTypeId,b.PayTypeName";
            }
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        /// <summary>
        /// 营销推广员销售汇总
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<DataTable> StatPromoterSaleAsync(StatPromoterSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("b.CTime>=@StartCTime");
            where.AppendWhere("b.CTime<@EndCTime");
            where.AppendWhereIf(input.PromoterId.HasValue, "b.PromoterId=@PromoterId");

            string groundRefundSql = $@"
UNION ALL
SELECT
b.PromoterId,
'项目退费' AS [Type],
b.TicketTypeID,
NULL AS RealPrice,
NULL AS SaleNum,
NULL AS SaleMoney,
NULL AS ReturnNum,
NULL AS ReturnMoney,
NULL AS RealNum,
SUM(-ABS(a.RealMoney)) AS RealMoney
FROM dbo.TM_TicketGroundSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_TicketSale b WITH(NOLOCK) ON b.ID=a.TicketID
{where}
AND b.CommitFlag=1
AND b.PromoterId IS NOT NULL
AND b.TicketStatusID<>11
AND a.PersonNum<0
GROUP BY b.PromoterId,b.TicketTypeID,a.RealPrice
";
            string sql = $@"
SELECT
b.PromoterId,
'门票' AS [Type],
b.TicketTypeID,
b.ReaPrice AS RealPrice,
SUM(CASE WHEN b.TicketStatusID=11 THEN 0 ELSE b.PersonNum END) AS SaleNum,
SUM(CASE WHEN b.TicketStatusID=11 THEN 0 ELSE ABS(b.ReaMoney) END) AS SaleMoney,
SUM(CASE WHEN b.TicketStatusID=11 THEN ABS(b.PersonNum) ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN b.TicketStatusID=11 THEN ABS(b.ReaMoney) ELSE 0 END) AS ReturnMoney,
SUM(CASE WHEN b.TicketStatusID=11 THEN -ABS(b.PersonNum) ELSE ABS(b.PersonNum) END) AS RealNum,
SUM(CASE WHEN b.TicketStatusID=11 THEN -ABS(b.ReaMoney) ELSE ABS(b.ReaMoney) END) AS RealMoney
FROM dbo.TM_TicketSale b WITH(NOLOCK)
{where}
AND b.CommitFlag=1
AND b.PromoterId IS NOT NULL
GROUP BY b.PromoterId,b.TicketTypeID,b.ReaPrice
{(input.IncludeGroundRefund ? groundRefundSql : string.Empty)}
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatByTradeSourceAsync(StatTicketSaleByTradeSourceInput input)
        {
            string statType = string.Empty;
            if (input.StatType == "1")
            {
                statType = "a.CDate,";
            }
            else if (input.StatType == "2")
            {
                statType = "a.CMonth,";
            }
            else if (input.StatType == "3")
            {
                statType = "a.CYear,";
            }
            string statTypeColumn = statType.IsNullOrEmpty() ? string.Empty : $"{statType.TrimEnd(',')} AS StatType,";

            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag=1");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.SalePointId.HasValue, "a.SalePointID=@SalePointId");
            where.AppendWhereIf(input.TradeSource.HasValue, "b.TradeSource=@TradeSource");
            where.AppendWhereIf(
                input.TicketTypeSearchGroupId.HasValue,
                "EXISTS(SELECT TOP 1 1 FROM dbo.TM_TicketTypeSearchGroupDetail WHERE TicketTypeID=a.TicketTypeID AND TicketTypeSearchGroupID=@TicketTypeSearchGroupId)");

            string sql = $@"
SELECT
{statTypeColumn}
ISNULL(b.TradeSource,1) AS TradeSource,
a.TicketTypeName,
a.ReaPrice,
SUM(a.TicketNum) AS TicketNum,
SUM(a.PersonNum) AS PersonNum,
SUM(a.ReaMoney) AS ReaMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID
{where}
GROUP BY {statType}b.TradeSource,a.TicketTypeName,a.ReaPrice
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatByTicketTypeClassAsync(StatTicketSaleByTicketTypeClassInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag=1");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.TicketTypeClassId.HasValue, "b.TicketTypeClassID=@TicketTypeClassId");

            string sql = $@"
SELECT
b.TicketTypeClassID,
a.TicketTypeID,
a.ReaPrice AS RealPrice,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.TicketNum END) AS SaleNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.PersonNum END) AS SalePersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.ReaMoney END) AS SaleMoney,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.TicketNum) ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.PersonNum) ELSE 0 END) AS ReturnPersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.ReaMoney) ELSE 0 END) AS ReturnMoney,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.TicketNum) ELSE ABS(a.TicketNum) END) AS RealNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.PersonNum) ELSE ABS(a.PersonNum) END) AS RealPersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.ReaMoney) ELSE ABS(a.ReaMoney) END) AS RealMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_TicketTypeClassDetail b WITH(NOLOCK) ON b.TicketTypeID=a.TicketTypeID
{where}
GROUP BY b.TicketTypeClassID,a.TicketTypeID,a.ReaPrice
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatByPayTypeAsync(StatTicketSaleByPayTypeInput input, IEnumerable<ComboboxItemDto<int>> payTypes)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("CTime>=@StartCTime");
            where.AppendWhere("CTime<@EndCTime");
            where.AppendWhere("CommitFlag=1");
            where.AppendWhere("StatFlag=1");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "TicketTypeID=@TicketTypeId");

            StringBuilder payTypeColumns = new StringBuilder();
            foreach (var payType in payTypes)
            {
                payTypeColumns.Append("[").Append(payType.Value).Append("],");
            }

            string sql = $@"
SELECT
*
FROM
(
	SELECT
	TicketTypeID,
	PayTypeID,
	SUM(ReaMoney) AS TotalMoney
	FROM dbo.TM_TicketSale WITH(NOLOCK)
    {where}
	GROUP BY TicketTypeID,PayTypeID
)x
PIVOT(SUM(x.TotalMoney) FOR x.PayTypeID IN ({payTypeColumns.ToString().TrimEnd(',')})) AS p
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatBySalePointAsync(StatTicketSaleBySalePointInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag=1");
            where.AppendWhereIf(input.ParkId.HasValue, "a.ParkID=@ParkId");
            where.AppendWhereIf(input.SalePointId.HasValue, "a.SalePointID=@SalePointId");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");

            string sql = $@"
SELECT
a.ParkID,
a.SalePointID,
a.TicketTypeID,
a.ReaPrice,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.TicketNum END) AS SaleNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.PersonNum END) AS SalePersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.ReaMoney END) AS SaleMoney,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.TicketNum) ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.PersonNum) ELSE 0 END) AS ReturnPersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.ReaMoney) ELSE 0 END) AS ReturnMoney,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.TicketNum) ELSE ABS(a.TicketNum) END) AS RealNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.PersonNum) ELSE ABS(a.PersonNum) END) AS RealPersonNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.ReaMoney) ELSE ABS(a.ReaMoney) END) AS RealMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
{where}
GROUP BY a.ParkID,a.SalePointID,a.TicketTypeID,a.ReaPrice
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatGroundSharingAsync(StatGroundSharingInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("b.CTime>=@StartCTime");
            where.AppendWhere("b.CTime<@EndCTime");
            where.AppendWhere("b.CommitFlag=1");
            where.AppendWhere("b.StatFlag=1");
            where.AppendWhere($"b.SalePointID<>{DefaultSalePoint.分销平台}");
            where.AppendWhereIf(input.SalePointId.HasValue, "b.SalePointID=@SalePointId");
            where.AppendWhereIf(input.TicketTypeId.HasValue, "b.TicketTypeID=@TicketTypeId");
            where.AppendWhereIf(input.GroundId.HasValue, "a.GroundID=@GroundId");

            string sql = $@"
SELECT
a.GroundID,
b.SalePointID,
b.TicketTypeID,
b.ReaPrice,
a.SharingRate,
SUM(b.PersonNum) AS PersonNum,
SUM(a.SharingMoney) AS SharingMoney
FROM dbo.TM_TicketSaleGroundSharing a WITH(NOLOCK)
LEFT JOIN dbo.TM_TicketSale b WITH(NOLOCK) ON a.TicketID=b.ID
{where}
GROUP BY a.GroundID,b.SalePointID,b.TicketTypeID,b.ReaPrice,a.SharingRate
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatJbAsync(StatJbInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<@EndCTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag=1");
            where.AppendWhereIf(input.ParkId.HasValue, "b.ParkID=@ParkId");
            where.AppendWhereIf(input.SalePointId.HasValue, "b.SalePointID=@SalePointId");
            where.AppendWhereIf(input.CashierId.HasValue, "b.CashierID=@CashierId");
            where.AppendWhereIf(input.HasShift.HasValue, "b.ShiftFlag=@HasShift");

            StringBuilder ticketWhere = new StringBuilder(where.ToString());
            ticketWhere.AppendWhere("a.TicketTypeTypeID<>9");

            StringBuilder tradeWhere = new StringBuilder(where.ToString());
            tradeWhere.AppendWhere("a.TotalMoney<>0");
            tradeWhere.AppendWhere("a.TradeTypeTypeID NOT IN(3,6)");
            tradeWhere.AppendWhere("a.TradeTypeID NOT IN(10,11,21,41)");

            string payTypeColumn = input.StatTicketByPayType ? "c.PayTypeID," : string.Empty;
            string payTypeJoin = input.StatTicketByPayType ? "LEFT JOIN dbo.TM_PayDetail c WITH(NOLOCK) ON a.TradeID=c.TradeID" : string.Empty;

            string sql = $@"
SELECT
{payTypeColumn}
b.TradeTypeID,
a.TicketTypeID,
a.ReaPrice AS RealPrice,
SUM(CASE WHEN a.TicketStatusID=11 THEN 0 ELSE a.PersonNum END) AS SaleNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN ABS(a.PersonNum) ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.PersonNum) ELSE a.PersonNum END) AS RealNum,
SUM(CASE WHEN a.TicketStatusID=11 THEN -ABS(a.TicketNum) ELSE a.TicketNum END) AS RealTicketNum,
SUM(a.ReaMoney) AS RealMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID
{payTypeJoin}
{ticketWhere}
GROUP BY {payTypeColumn}b.TradeTypeID,a.TicketTypeID,a.ReaPrice
UNION ALL
SELECT
{payTypeColumn}
a.TradeTypeID,
NULL AS TicketTypeID,
ABS(a.TotalMoney) AS RealPrice,
SUM(CASE WHEN a.TotalMoney>=0 THEN 1 ELSE 0 END) AS SaleNum,
SUM(CASE WHEN a.TotalMoney<0 THEN 1 ELSE 0 END) AS ReturnNum,
SUM(CASE WHEN a.TotalMoney<0 THEN -1 ELSE 1 END) AS RealNum,
NULL AS RealTicketNum,
SUM(a.TotalMoney) AS RealMoney
FROM dbo.TM_TradeDetail a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID
{payTypeJoin}
{tradeWhere}
GROUP BY {payTypeColumn}a.TradeTypeID,a.TotalMoney
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatByCustomerAsync(StatTicketSaleByCustomerInput input)
        {
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.AppendWhereIf(input.StartCTime.HasValue, "a.CTime>=@StartCTime");
            whereBuilder.AppendWhereIf(input.EndCTime.HasValue, "a.CTime<@EndCTime");
            whereBuilder.AppendWhereIf(input.StartTravelDate.HasValue, "a.STime>=@StartTravelDate");
            whereBuilder.AppendWhereIf(input.EndTravelDate.HasValue, "a.STime<=@EndTravelDate");
            whereBuilder.AppendWhere("a.CustomerID IS NOT NULL");
            whereBuilder.AppendWhereIf(input.CustomerId.HasValue, "a.CustomerID=@CustomerId");
            whereBuilder.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            whereBuilder.AppendWhereIf(input.TradeSource.HasValue, "b.TradeSource=@TradeSource");

            string sql = $@"
SELECT
a.CustomerID,
a.TicketTypeID,
a.ReaPrice,
SUM(a.TicketNum) AS TicketNum,
SUM(a.PersonNum) AS PersonNum,
SUM(a.ReaMoney) AS ReaMoney
FROM dbo.TM_TicketSale a WITH(NOLOCK)
LEFT JOIN dbo.TM_Trade b WITH(NOLOCK) ON b.ID=a.TradeID
{whereBuilder}
GROUP BY a.CustomerID,a.TicketTypeID,a.ReaPrice
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatCzkSaleAsync(StatCzkSaleInput input)
        {
            StringBuilder whereBuilder = new StringBuilder();
            whereBuilder.AppendWhere("a.CzkOpTypeID=1");
            whereBuilder.AppendWhere("b.TicketTypeTypeID<>9");
            whereBuilder.AppendWhere("a.CzkRechargeTypeID IN (1,2,3)");
            whereBuilder.AppendWhereIf(input.StartCtime.HasValue, "a.Ctime >= @StartCtime");
            whereBuilder.AppendWhereIf(input.EndCtime.HasValue, "a.Ctime <= @EndCtime");
            whereBuilder.AppendWhereIf(input.TicketTypeId.HasValue, "a.TicketTypeID=@TicketTypeId");
            whereBuilder.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            string sql = $@"
SELECT
 a.CashierName
,a.TicketTypeName
,(CASE WHEN a.CzkRechargeTypeID=3 THEN '结算' ELSE a.CzkRechargeTypeName END) AS CzkRechargeTypeName
,(CASE WHEN a.CzkRechargeTypeID=1 THEN a.RealPrice ELSE a.RechargeCardMoney END) AS RechargeMoney
,1 AS TotalNum
FROM MM_CzkDetail a WITH(NOLOCK)
LEFT JOIN dbo.TM_TicketType b WITH(NOLOCK) ON a.TicketTypeID=b.ID
{whereBuilder}
ORDER BY a.CzkRechargeTypeID,a.CashierName,a.TicketTypeName
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatCzkSaleJbAsync(StatJbInput input)
        {
            StringBuilder whereCzkSale = new StringBuilder(" where cd.CommitFlag=1 and cd.CzkOpTypeID=1 and cd.CzkRechargeTypeID <> 4 and cd.AccountTypeID = 1 ");
            whereCzkSale.AppendWhere("cd.CTime>=@StartCTime");
            whereCzkSale.AppendWhere("cd.CTime<@EndCTime");
            whereCzkSale.AppendWhereIf(input.SalePointId.HasValue, "cd.SalePointID=@SalePointId");
            whereCzkSale.AppendWhereIf(input.CashierId.HasValue, "cd.CashierID=@CashierId");
            whereCzkSale.AppendWhereIf(input.HasShift.HasValue, "td.ShiftFlag=@HasShift");
            StringBuilder whereOtherTrade = new StringBuilder(" where tdd.CommitFlag=1 and tdd.TotalMoney <> 0 and tdd.TradeTypeTypeID = 3 and tdd.TradeTypeID not in(31,37)");
            whereOtherTrade.AppendWhere("td.CTime>=@StartCTime");
            whereOtherTrade.AppendWhere("td.CTime<@EndCTime");
            whereOtherTrade.AppendWhereIf(input.SalePointId.HasValue, "td.SalePointID=@SalePointId");
            whereOtherTrade.AppendWhereIf(input.CashierId.HasValue, "td.CashierID=@CashierId");
            whereOtherTrade.AppendWhereIf(input.HasShift.HasValue, "td.ShiftFlag=@HasShift");
            StringBuilder whereQzjsTrade = new StringBuilder(" where td.CommitFlag = 1 and td.TradeTypeID = 37");
            whereQzjsTrade.AppendWhere("td.CTime>=@StartCTime");
            whereQzjsTrade.AppendWhere("td.CTime<@EndCTime");
            whereQzjsTrade.AppendWhereIf(input.SalePointId.HasValue, "td.SalePointID=@SalePointId");
            whereQzjsTrade.AppendWhereIf(input.CashierId.HasValue, "td.CashierID=@CashierId");
            whereQzjsTrade.AppendWhereIf(input.HasShift.HasValue, "td.ShiftFlag=@HasShift");

            string sql = $@"
select
 x.TicketTypeName
,x.TradeTypeName
,x.Num
,x.PayMoney
from
(
	select 
	 tt.Name as TicketTypeName	
	,crt.Name as TradeTypeName
	,cd.CzkRechargeTypeID 
	,count(*) as Num	
	,sum(isnull(cd.RechargeCardMoney,0)) as PayMoney	
	from MM_CzkDetail cd
	join TM_Trade td on cd.TradeID = td.ID	
	left join MM_CzkRechargeType crt on cd.CzkRechargeTypeID = crt.ID	
	left join TM_TicketType tt on cd.TicketTypeID = tt.ID
	{whereCzkSale}
    group by tt.Name,crt.Name,cd.CzkRechargeTypeID
	union all
	select 	
	 '----'
	,tdd.TradeTypeName
	,tdd.TradeTypeID
	,sum(case when tdd.TotalMoney > 0 or tdd.TradeTypeID = 301 then 1 else -1 end) 
	,sum(tdd.TotalMoney) as PayMoney	
    
	from TM_TradeDetail tdd 	
	join TM_Trade td on tdd.TradeID=td.ID		
	{whereOtherTrade}
    group by tdd.TradeTypeID,tdd.TradeTypeName 
    union all
    select 
    '----'
    ,'储值卡强制结算收入'
    ,1000
    ,sum(1)
    ,sum(td.TotalMoney) as PayMoney
    from TM_Trade td
    {whereQzjsTrade}
)x
where x.CzkRechargeTypeID <> 1000 or x.PayMoney > 0
order by TicketTypeName desc,x.TradeTypeName asc,x.CzkRechargeTypeID desc";

            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction, TimeoutSeconds);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}
