using Dapper;
using Egoal.Application.Services.Dto;
using Egoal.EntityFrameworkCore;
using Egoal.Extensions;
using Egoal.Tickets.Dto;
using Egoal.Wares.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Wares
{
    public class WareRepository : EfCoreRepositoryBase<WareType>, IWareRepository
    {
        public WareRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task<PagedResultDto<WareListDto>> QueryWareAsync(QueryWareInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(!input.Name.IsNullOrEmpty(), $"a.Name LIKE '{input.Name}%'");
            where.AppendWhereIf(!input.WareCode.IsNullOrEmpty(), $"a.WareCode LIKE '{input.WareCode}%'");
            where.AppendWhereIf(!input.Zjf.IsNullOrEmpty(), $"a.Zjf LIKE '{input.Zjf}%'");
            where.AppendWhereIf(!input.BarCode.IsNullOrEmpty(), $"a.BarCode LIKE '{input.BarCode}%'");
            where.AppendWhereIf(input.WareTypeId.HasValue, "a.WareTypeId=@WareTypeId");
            where.AppendWhereIf(input.MerchantId.HasValue, "a.MerchantId=@MerchantId");
            where.AppendWhereIf(input.SupplierId.HasValue, "a.SupplierId=@SupplierId");

            string sql = $@"
SELECT 
a.ID
,a.Name 
,a.WareCode 
,a.Barcode 
,a.Zjf 
,a.SortCode 
,a.WareTypeName 
,a.WareRsTypeName 
,a.WareUnit 
,a.WareStandard 
,a.CostPrice
,a.RetailPrice 
,a.RentPrice 
,a.YaJin 
,a.RentTypeName 
,a.FreeJsMinutes 
,a.FeeJsUnit 
,a.RentJsPrice 
,a.SaleFlag 
,b.MerchantName 
,a.WareProducter 
,a.SupplierName 
,a.WareColour 
,a.StockAlarmNum 
,a.Memo 
,a.NeedWareHouseFlag
,ROW_NUMBER() OVER (ORDER BY a.SortCode) AS RowNum
FROM [WM_Ware] a WITH(NOLOCK)
LEFT JOIN WM_Merchant b ON a.MerchantID=b.ID
{where}";
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
FROM [WM_Ware] a WITH(NOLOCK)
LEFT JOIN WM_Merchant b ON a.MerchantID=b.ID
{where}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var items = await Connection.QueryAsync<WareListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<WareListDto>(count, items.ToList());
            }
            else
            {
                var items = await Connection.QueryAsync<WareListDto>(sql, input, Transaction);

                return new PagedResultDto<WareListDto>(items.Count(), items.ToList());
            }
        }

        public async Task<PagedResultDto<WareIODetailListDto>> QueryWareIODetailAsync(QueryWareIODetailInput input)
        {
            if (!string.IsNullOrEmpty(input.CzkCardNo))
            {
                var accountId = await GetCardLastMemberAccountID(input.CzkCardNo);
                if (string.IsNullOrEmpty(accountId))
                {
                    return new PagedResultDto<WareIODetailListDto>();
                }
            }

            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime>=@SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime<@ECTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhereIf(!input.WareName.IsNullOrEmpty(), $"a.WareName LIKE '{input.WareName}%'");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), $"a.ListNo LIKE '{input.ListNo}%'");
            where.AppendWhereIf(input.WareTypeId.HasValue, "b.WareTypeId=@WareTypeId");
            where.AppendWhereIf(input.TradeTypeId.HasValue, "a.TradeTypeId=@TradeTypeId");
            where.AppendWhereIf(input.TradeTypeId.HasValue, "((a.TradeTypeID>=61 AND a.TradeTypeID<=64) OR a.TradeTypeID = 603)");
            where.AppendWhereIf(input.MerchantId.HasValue, "d.MerchantId=@MerchantId");
            where.AppendWhereIf(input.WareShopId.HasValue, "a.WareShopId=@WareShopId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "a.CashPcID=@CashPcId");

            string sql = $@"
SELECT 
 a.[WareName] 
,a.[TradeTypeName] 
,a.[ListNo] 
,a.[CostPrice] 
,a.[RetailPrice] 
,a.[RentPrice] 
,a.[YaJin] 
,a.[IONum] 
,a.[ReaMoney] 
,a.[DiscountTypeName] 
,a.[CashierName] 
,a.[CashPCName] 
,a.[WareShopName] 
,a.[Memo] 
,CONVERT(VARCHAR(19),a.CTime,120) AS CTime 
,NULL AS visiable
,a.[TradeTypeID]
,ROW_NUMBER() OVER (ORDER BY a.CTime DESC) AS RowNum
FROM [WM_WareIODetail] a WITH(NOLOCK)
LEFT JOIN [WM_Ware] b WITH(NOLOCK) ON a.WareID = b.ID
LEFT JOIN WM_Shop d WITH(NOLOCK) ON a.WareShopID=d.ID
{where}";
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
FROM [WM_WareIODetail] a WITH(NOLOCK)
LEFT JOIN [WM_Ware] b WITH(NOLOCK) ON a.WareID = b.ID
LEFT JOIN WM_Shop d WITH(NOLOCK) ON a.WareShopID=d.ID
{where}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var trades = await Connection.QueryAsync<WareIODetailListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<WareIODetailListDto>(count, trades.ToList());
            }
            else
            {
                var trades = await Connection.QueryAsync<WareIODetailListDto>(sql, input, Transaction);

                return new PagedResultDto<WareIODetailListDto>(trades.Count(), trades.ToList());
            }
        }

        public async Task<PagedResultDto<WareTradeListDto>> QueryWareTradeAsync(QueryWareTradeInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.SCTime.HasValue, $"b.CTime>=@SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"b.CTime<@ECTime");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), $"b.ListNo LIKE '%{input.ListNo}%'");
            where.AppendWhereIf(!input.CzkCardNo.IsNullOrEmpty(), $"b.CzkCardNo = @CzkCardNo");
            where.AppendWhereIf(input.TradeTypeTypeId.HasValue, $"b.TradeTypeTypeId = @TradeTypeTypeId");
            where.AppendWhereIf(input.TradeTypeId.HasValue, "b.TradeTypeId=@TradeTypeId");
            where.AppendWhereIf(input.CashierId.HasValue, "b.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "b.CashPcID=@CashPcId");
            where.AppendWhereIf(input.MerchantId.HasValue, "c.MerchantId=@MerchantId");
            where.AppendWhereIf(input.ShopTypeId.HasValue, "c.ShopTypeId = @WareShopTypeId");
            where.AppendWhereIf(input.ShopId.HasValue, "b.ShopId=@ShopId");
            where.AppendWhereIf(!input.Memo.IsNullOrEmpty(), $"b.Memo = @Memo");

            string sql = $@"
SELECT
 convert(varchar(19),a.CTime,120) AS CTime
,b.CzkCardNo 
,b.ListNo 
,b.TradeTypeName 
,a.PayMoney 
,a.PayTypeName 
,a.CurrencyName 
,a.WbPayMoney 
,b.CashierName 
,b.CashPCName 
,b.ShopName 
,b.ParkName 
,b.MemberName 
,b.CustomerName 
,b.GuiderName 
,b.Memo 
,null AS Visible
,b.ID 
,b.YaJin 
,b.SalesmanName 
,b.ApproverName 
,b.AreaName 
,ROW_NUMBER() OVER (ORDER BY a.CTime DESC) AS RowNum
FROM TM_PayDetail a WITH(NOLOCK)
LEFT JOIN TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID 
LEFT JOIN WM_Shop c WITH(NOLOCK) ON b.ShopID=c.ID
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
ORDER BY ctime desc
";
            string countSql = $@"
SELECT
Count(*)
FROM TM_PayDetail a WITH(NOLOCK) 
LEFT JOIN TM_Trade b WITH(NOLOCK) ON a.TradeID=b.ID 
LEFT JOIN WM_Shop c WITH(NOLOCK) ON b.ShopID=c.ID
{where}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var trades = await Connection.QueryAsync<WareTradeListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<WareTradeListDto>(count, trades.ToList());
            }
            else
            {
                var trades = await Connection.QueryAsync<WareTradeListDto>(sql, input, Transaction);

                return new PagedResultDto<WareTradeListDto>(trades.Count(), trades.ToList());
            }
        }

        /// <summary>
        /// 获取储值卡账户号
        /// </summary>
        /// <param name="cardNo"></param>
        /// <returns></returns>
        public async Task<string> GetCardLastMemberAccountID(string czkCardNo)
        {
            StringBuilder where = new StringBuilder();

            string sql = @"
DECLARE @MemberAccountID VARCHAR(50)
IF EXISTS(SELECT TOP 1 1 FROM MM_MemberCard WHERE CardNo=@czkCardNo)
BEGIN
	SELECT @MemberAccountID=MemberAccountID FROM MM_MemberCard WHERE CardNo=@czkCardNo
END
ELSE
BEGIN
	SELECT @MemberAccountID=MemberAccountID FROM MM_CzkDetail WHERE CardNo=@czkCardNo ORDER BY ID DESC
END
SELECT @MemberAccountID
";
            var reader = await Connection.ExecuteScalarAsync<string>(sql, new { czkCardNo }, Transaction);
            return reader;
        }

        public async Task<DataTable> StatWareTradeAsync(StatWareTradeInput input, IEnumerable<ComboboxItemDto<int>> payTypes)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("b.CommitFlag=1");
            where.AppendWhere("b.StatFlag = 1");
            where.AppendWhereIf(input.SCTime.HasValue, $"b.CTime >= @SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"b.CTime < @ECTime");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), $"b.ListNo LIKE '%{input.ListNo}%'");
            where.AppendWhereIf(!input.Memo.IsNullOrEmpty(), $"b.Memo = @Memo");
            where.AppendWhereIf(input.TradeTypeTypeId.HasValue, $"b.TradeTypeTypeId = @TradeTypeTypeId");
            where.AppendWhereIf(input.TradeTypeId.HasValue, "b.TradeTypeId=@TradeTypeId");
            where.AppendWhereIf(input.CashierId.HasValue, "b.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "b.CashPcID=@CashPcId");
            where.AppendWhereIf(input.MerchantId.HasValue, "c.MerchantId=@MerchantId");
            where.AppendWhereIf(input.ShopTypeId.HasValue, "c.ShopTypeId = @ShopTypeId");
            where.AppendWhereIf(input.ShopId.HasValue, "b.ShopId=@ShopId");

            StringBuilder payTypeColumns = new StringBuilder();
            foreach (var payType in payTypes)
            {
                payTypeColumns.Append("[").Append(payType.Value).Append("],");
            }

            string statColumn = input.statColumns[input.StatTypeId];

            string sql = $@"
SELECT
*
From
(
    SELECT
    {statColumn},
    a.PayTypeId,
    SUM(a.PayMoney) AS PayMoney
    FROM TM_PayDetail a WITH(NOLOCK)
    LEFT JOIN TM_Trade b WITH(NOLOCK) ON a.TradeId = b.Id
    LEFT JOIN WM_Shop c ON b.ShopId = c.Id
    {where}
    GROUP BY {statColumn},a.PayTypeId
)x
PIVOT(SUM(x.PayMoney) FOR x.PayTypeId IN ({payTypeColumns.ToString().TrimEnd(',')})) AS p";
            IDataReader reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareSaleByWareTypeAsync(StatWareSaleByWareTypeInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.TradeTypeID between 61 and 62");
            where.AppendWhere("a.CommitFlag=1");
            where.AppendWhere("a.StatFlag = 1");
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime >= @SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime < @ECTime");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), $"a.ListNo LIKE '%{input.ListNo}%'");
            where.AppendWhereIf(input.WareTypeId.HasValue, "b.WareTypeId = @WareTypeId");
            where.AppendWhereIf(input.MerchantId.HasValue, "b.MerchantId=@MerchantId");
            where.AppendWhereIf(input.WareShopId.HasValue, "a.WareShopId=@WareShopId");
            where.AppendWhereIf(input.SupplierId.HasValue, "b.SupplierId = @SupplierId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "a.CashPcID=@CashPcId");

            string sql = $@"

SELECT
x.商品类型,
x.销售数量,
x.销售金额,
x.退货数量,
x.退货金额,
x.销售数量-x.退货数量 AS 实售数量,
x.销售金额-x.退货金额 AS 实售金额
FROM
(
    SELECT 
    b.WareTypeID,
	c.Name AS 商品类型,
    SUM(case when a.TradeTypeID=61 THEN -IONum ELSE 0 END) AS 销售数量,
    SUM(case when a.TradeTypeID=61 THEN ReaMoney ELSE 0 END) AS 销售金额,
    SUM(case when a.TradeTypeID=62 THEN IONum ELSE 0 END) AS 退货数量,
    SUM(case when a.TradeTypeID=62 THEN -ReaMoney ELSE 0 END) AS 退货金额
    FROM WM_WareIODetail a WITH(nolock)
    LEFT JOIN WM_Ware b WITH(nolock) ON b.ID=a.WareID
	LEFT JOIN WM_WareType c ON b.WareTypeId = c.Id
    {where}
    GROUP BY b.WareTypeID,c.Name
)x";
            IDataReader reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareRentSaleRentAsync(StatWareRentSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.TradeTypeId IN ('63','64','603')");
            where.AppendWhere("a.CommitFlag='1'");
            where.AppendWhere("a.StatFlag='1'");
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime>=@SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime<@ECTime");
            where.AppendWhereIf(input.WareTypeTypeId.HasValue, "c.WareTypeTypeId=@WareTypeTypeId");
            where.AppendWhereIf(input.MerchantId.HasValue, "b.MerchantId=@MerchantId");
            where.AppendWhereIf(input.ShopTypeId.HasValue, "e.ShopTypeId=@ShopTypeId");
            where.AppendWhereIf(input.WareShopId.HasValue, "a.WareShopId=@WareShopId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "a.CashPcID=@CashPcId");

            string sql = $@"
SELECT 
 x.CDate AS CDate
,dbo.FWMGetWareName(x.WareID) AS WareName
,x.出租数量 AS RentNum
,x.出租金额 AS RentMoney
,x.退租数量 AS ReturnNum
,x.退租金额 AS ReturnMoney
,x.归还数量 AS RevertNum
,x.归还金额 AS RevertMoney
,x.出租数量-x.归还数量-x.退租数量 AS NotRevertNum
,x.出租金额-x.归还金额-x.退租金额 AS RentMoneyTotal
,x.未退押金 AS NotReturnMoney
 from
(
	SELECT 
     CDate
	,WareID
	,SUM(case when TradeTypeID = 63 THEN -IONum ELSE 0 end) AS 出租数量
	,SUM(case when TradeTypeID = 63 THEN ReaMoney ELSE 0 end) AS 出租金额
    ,SUM(case when TradeTypeID = 603 THEN IONum ELSE 0 end) AS 退租数量
    ,SUM(case when TradeTypeID = 603 THEN -ReaMoney ELSE 0 end) AS 退租金额
	,SUM(case when TradeTypeID = 64 THEN IONum ELSE 0 end) AS 归还数量
	,SUM(case when TradeTypeID = 64 THEN -ReaMoney ELSE 0 end) AS 归还金额
    ,SUM(a.YaJin) AS 未退押金
	FROM WM_WareIODetail a WITH(readpast)
    LEFT JOIN WM_Ware b ON a.WareID = b.ID 
    LEFT JOIN WM_WareType c WITH(nolock) ON b.wareTypeID=c.ID
    LEFT JOIN WM_Shop d WITH(nolock) ON a.WareShopID=d.ID
    {where}
	GROUP BY CDate,WareID,d.name
) x
ORDER BY CDate desc
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareRentSaleSaleAsync(StatWareRentSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.TradeTypeId>='61'");
            where.AppendWhere("a.TradeTypeId<='62'");
            where.AppendWhere("a.CommitFlag='1'");
            where.AppendWhere("a.StatFlag='1'");
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime >= @SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime < @ECTime");
            where.AppendWhereIf(!input.WareName.IsNullOrEmpty(), $"a.WareName LIKE '%{input.WareName}%'");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), $"a.ListNo LIKE '%{input.ListNo}%'");
            where.AppendWhereIf(input.WareTypeTypeId.HasValue, "c.WareTypeTypeId=@WareTypeTypeId");
            where.AppendWhereIf(input.WareTypeId.HasValue, "b.WareTypeId=@WareTypeId");
            where.AppendWhereIf(input.MerchantId.HasValue, "b.MerchantId=@MerchantId");
            where.AppendWhereIf(input.ShopTypeId.HasValue, "e.ShopTypeId=@ShopTypeId");
            where.AppendWhereIf(input.WareShopId.HasValue, "a.WareShopId=@WareShopId");
            where.AppendWhereIf(input.SupplierId.HasValue, "b.SupplierId=@SupplierId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "a.CashPcID=@CashPcId");

            string sql = $@"
SELECT 
 x.CDate AS CDate
,dbo.FWMGetWareName(x.WareID) AS WareName
,x.商品类型 AS WareTypeName
,x.ReaPrice AS ReaPrice
,x.销售数量 AS SaleNum
,x.销售金额 AS SaleMoney
,x.退货数量 AS ReturnNum
,x.退货金额 AS ReturnMoney
,x.销售数量-x.退货数量 AS RealNum
,x.销售金额-x.退货金额 AS RealMoney
 from
(
	SELECT 
     CDate
	,WareID
	,ReaPrice
	,SUM(case when TradeTypeID = 61 THEN -IONum ELSE 0 end) AS 销售数量
	,SUM(case when TradeTypeID = 61 THEN ReaMoney ELSE 0 end) AS 销售金额
	,SUM(case when TradeTypeID = 62 THEN IONum ELSE 0 end) AS 退货数量
	,SUM(case when TradeTypeID = 62 THEN -ReaMoney ELSE 0 end) AS 退货金额
    ,c.Name AS 商品类型
	FROM WM_WareIODetail a WITH(readpast)
    LEFT JOIN WM_Ware b ON a.WareID = b.ID 
    LEFT JOIN wm_waretype c WITH(nolock) ON b.WareTypeID=c.ID
    LEFT JOIN WM_Shop e WITH(nolock) ON a.WareShopID=e.ID
    {where}
	GROUP BY CDate,WareID,ReaPrice,c.Name
) x
ORDER BY CDate desc
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareTradeTotal(StatWareTradeTotalInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.TradeTypeId IN ('61','62','63','64','603')");
            where.AppendWhere("a.CommitFlag='1'");
            where.AppendWhere("a.StatFlag='1'");
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime >= @SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime < @ECTime");
            where.AppendWhereIf(input.ShopId.HasValue, "a.ShopId=@ShopId");

            string sql = $@"
SELECT 
 ShopName AS WareShopName
--,TradeTypeName AS TradeTypeName
,SUM(TotalMoney) AS TotalMoney
FROM TM_Trade a WITH(readpast)
{where}
GROUP BY a.ShopName 
ORDER BY a.ShopName
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareSaleAsync(StatWareSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.TradeTypeId>='61'");
            where.AppendWhere("a.TradeTypeId<='62'");
            where.AppendWhere("a.CommitFlag='1'");
            where.AppendWhere("a.StatFlag='1'");
            where.AppendWhereIf(input.SCTime.HasValue, $"a.CTime >= @SCTime");
            where.AppendWhereIf(input.ECTime.HasValue, $"a.CTime < @ECTime");
            where.AppendWhereIf(!input.WareName.IsNullOrEmpty(), $"a.WareName LIKE '%{input.WareName}%'");
            where.AppendWhereIf(input.WareShopId.HasValue, "a.WareShopId=@WareShopId");
            where.AppendWhereIf(input.CashierId.HasValue, "a.CashierID=@CashierId");
            where.AppendWhereIf(input.CashPcId.HasValue, "a.CashPcID=@CashPcId");

            string sql = $@"
SELECT 
 CDate 
,WareShopName
,WareName
,RetailPrice AS WarePrice
,abs(SUM(ISNULL(IONum,0))) AS WareNum
,SUM(ReaMoney) AS TotalMoney
FROM WM_WareIODetail a WITH(readpast)
{where}
GROUP BY a.CDate,a.WareShopName,a.WareName,a.RetailPrice
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<DataTable> StatWareSaleShiftAsync(StatJbInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("wi.CTime>=@StartCTime");
            where.AppendWhere("wi.CTime<=@EndCTime");
            where.AppendWhere("wi.CommitFlag = '1' ");
            where.AppendWhere("wi.TradeTypeID in(61,62,63,64,603)");
            where.AppendWhereIf(input.ParkId.HasValue, "td.ParkID=@ParkId");
            where.AppendWhereIf(input.SalePointId.HasValue, "td.SalePointID=@SalePointId");
            where.AppendWhereIf(input.CashierId.HasValue, "td.CashierID=@CashierId");
            where.AppendWhereIf(input.HasShift.HasValue, "td.ShiftFlag=@HasShift");

            string sql = $@"
SELECT 
 (CASE WHEN wi.TradeTypeID in(61,62) THEN '销售' ELSE '租赁' END) AS TypeName
,tdt.Name AS TradeTypeName	
,SUM(-wi.IONum) AS Num
,SUM(ISNULL(wi.ReaMoney,0)-ISNULL(wi.YaJin,0)) AS RealMoney	
,SUM(ISNULL(wi.YaJin,0)) AS YaJin
FROM WM_WareIODetail wi
JOIN TM_Trade td ON wi.TradeID = td.ID
LEFT JOIN TM_TradeType tdt ON td.TradeTypeID=tdt.ID
{where}
GROUP BY tdt.Name,wi.TradeTypeID
ORDER BY TypeName,wi.TradeTypeID
";
            IDataReader reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }
    }
}
