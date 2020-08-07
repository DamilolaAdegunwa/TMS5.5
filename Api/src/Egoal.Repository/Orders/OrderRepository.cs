using Dapper;
using Egoal.Application.Services.Dto;
using Egoal.EntityFrameworkCore;
using Egoal.Extensions;
using Egoal.Orders.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class OrderRepository : EfCoreRepositoryBase<Order, string>, IOrderRepository
    {
        public OrderRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public async Task<Order> GetByIdAsync(string id)
        {
            var order = await Context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(detail => detail.OrderGroundChangCis)
                .Include(o => o.OrderDetails)
                    .ThenInclude(detail => detail.OrderTourists)
                .FirstOrDefaultAsync(o => o.Id == id);

            return order;
        }

        public async Task<int> GetMemberBuyQuantityAsync(Guid memberId, int ticketTypeId, DateTime startTime, DateTime endTime)
        {
            StringBuilder whereBuilder = new StringBuilder(" ");
            whereBuilder.AppendWhere("a.MemberID=@memberId");
            whereBuilder.AppendWhere("a.TicketTypeID=@ticketTypeId");

            string sql = $@"
SELECT
SUM(x.Quantity)
FROM
(
	SELECT
	SUM(a.PersonNum) AS Quantity
	FROM dbo.TM_TicketSale a
    WHERE (a.SDate BETWEEN @startTime AND @endTime)
	{whereBuilder}
	UNION ALL
	SELECT
	SUM(a.SurplusNum) AS Quantity
	FROM dbo.OM_OrderDetail a WITH(UPDLOCK)
	JOIN dbo.OM_Order b ON b.ListNo=a.ListNo
    WHERE (b.ETime BETWEEN @startTime AND @endTime)
	{whereBuilder}
	AND b.PayFlag=0
	AND b.OrderStatusID<>5
)x
";
            return await Connection.ExecuteScalarAsync<int>(sql, new { memberId, ticketTypeId, startTime, endTime }, Transaction);
        }

        public async Task<int> GetCertBuyQuantityAsync(string certNo, DateTime startTime, DateTime endTime)
        {
            string sql = @"
SELECT
SUM(x.Quantity)
FROM
(
	SELECT
	COUNT(*) AS Quantity
	FROM dbo.TM_TicketSaleBuyer a
	LEFT JOIN dbo.TM_TicketSale b ON b.ID=a.TicketID
	WHERE a.CertNo=@certNo
    AND a.SDate BETWEEN @startTime AND @endTime
    AND (b.TicketStatusID<=9 OR b.TicketStatusID IS NULL)
	UNION ALL
	SELECT
	COUNT(*) AS Quantity
	FROM dbo.OM_OrderTourist a WITH(UPDLOCK)
	JOIN dbo.OM_OrderDetail b ON b.ID=a.OrderDetailId
	JOIN dbo.OM_Order c ON c.ListNo=b.ListNo
	WHERE c.ETime BETWEEN @startTime AND @endTime
    AND a.CertNo=@certNo
    AND c.PayFlag=0
    AND c.OrderStatusID<>5
)x
";
            return await Connection.ExecuteScalarAsync<int>(sql, new { certNo, startTime, endTime }, Transaction);
        }

        public async Task<bool> HasExchangedAsync(string listNo)
        {
            string sql = @"
SELECT TOP 1 1
FROM dbo.TM_TicketSale WITH(NOLOCK)
WHERE OrderListNo=@listNo
AND (HasExchanged=1 OR PrintNum>0)
";
            return await Connection.ExecuteScalarAsync<string>(sql, new { listNo }, Transaction) == "1";
        }

        public async Task<DateTime?> GetOrderCheckInTimeAsync(string listNo)
        {
            string sql = @"
SELECT TOP 1
a.CTime
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
JOIN dbo.TM_TicketSale b WITH(NOLOCK) ON b.ID=a.TicketID
WHERE a.InOutFlag=1
AND b.OrderListNo=@listNo
";
            return await Connection.ExecuteScalarAsync<DateTime?>(sql, new { listNo }, Transaction);
        }

        public async Task<DateTime?> GetOrderCheckOutTimeAsync(string listNo)
        {
            string sql = @"
SELECT TOP 1
a.CTime
FROM dbo.TM_TicketCheck a WITH(NOLOCK)
JOIN dbo.TM_TicketSale b WITH(NOLOCK) ON b.ID=a.TicketID
WHERE a.InOutFlag=0
AND b.OrderListNo=@listNo
ORDER BY a.CTime DESC
";
            return await Connection.ExecuteScalarAsync<DateTime?>(sql, new { listNo }, Transaction);
        }

        public async Task<PagedResultDto<OrderListDto>> GetOrdersAsync(GetOrdersInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhereIf(input.StartCTime.HasValue, "a.CTime>=@StartCTime");
            where.AppendWhereIf(input.EndCTime.HasValue, "a.CTime<@EndCTime");
            where.AppendWhereIf(!input.StartTravelDate.IsNullOrEmpty(), "a.ETime>=@StartTravelDate");
            where.AppendWhereIf(!input.EndTravelDate.IsNullOrEmpty(), "a.ETime<=@EndTravelDate");
            where.AppendWhereIf(input.CustomerId.HasValue, "a.CustomerID=@CustomerId");
            where.AppendWhereIf(input.PromoterId.HasValue, "a.PromoterId=@PromoterId");
            where.AppendWhereIf(!input.ListNo.IsNullOrEmpty(), "(a.ListNo=@ListNo OR a.ThirdPartyPlatformOrderID=@ListNo)");
            where.AppendWhereIf(input.OrderStatus.HasValue, "a.OrderStatusID=@OrderStatus");
            where.AppendWhereIf(input.ConsumeStatus.HasValue, "a.ConsumeStatus=@ConsumeStatus");
            where.AppendWhereIf(input.RefundStatus.HasValue, "a.RefundStatus=@RefundStatus");
            where.AppendWhereIf(input.OrderType.HasValue, "a.OrderTypeID=@OrderType");
            where.AppendWhereIf(!input.ContactName.IsNullOrEmpty(), "(a.YdrName=@ContactName OR EXISTS(SELECT TOP 1 1 FROM dbo.OM_OrderTourist b WITH(NOLOCK) JOIN dbo.OM_OrderDetail c WITH(NOLOCK) ON c.ID=b.OrderDetailId WHERE c.ListNo=a.ListNo AND b.Name=@ContactName))");
            where.AppendWhereIf(!input.ContactMobile.IsNullOrEmpty(), "(a.Mobile=@ContactMobile OR EXISTS(SELECT TOP 1 1 FROM dbo.OM_OrderTourist b WITH(NOLOCK) JOIN dbo.OM_OrderDetail c WITH(NOLOCK) ON c.ID=b.OrderDetailId WHERE c.ListNo=a.ListNo AND b.Mobile=@ContactMobile))");
            where.AppendWhereIf(!input.ContactCertNo.IsNullOrEmpty(), "(a.CertNo=@ContactCertNo OR EXISTS(SELECT TOP 1 1 FROM dbo.OM_OrderTourist b WITH(NOLOCK) JOIN dbo.OM_OrderDetail c WITH(NOLOCK) ON c.ID=b.OrderDetailId WHERE c.ListNo=a.ListNo AND b.CertNo=@ContactCertNo))");
            where.AppendWhereIf(!input.Remark.IsNullOrEmpty(), $"a.Memo LIKE '{input.Remark}%'");
            if (input.HasCustomer.HasValue)
            {
                if (input.HasCustomer.Value)
                {
                    where.AppendWhere("a.CustomerID IS NOT NULL");
                }
                else
                {
                    where.AppendWhere("a.CustomerID IS NULL");
                }
            }

            string sql = $@"
SELECT
a.ListNo,
a.OrderTypeID,
a.OrderStatusID,
a.ConsumeStatus,
a.RefundStatus,
a.ETime AS TravelDate,
a.PayFlag,
a.TotalMoney,
a.TotalNum,
a.CollectNum,
a.UsedNum,
a.ReturnNum,
a.SurplusNum,
a.MemberName,
a.CustomerName,
a.GuiderName,
a.ExplainerId,
a.ExplainerTimeId,
a.PromoterId,
a.YdrName,
a.Mobile,
a.CertNo,
a.JidiaoName,
a.JidiaoMobile,
a.ThirdPartyPlatformOrderID AS ThirdListNo,
a.Memo,
a.CTime,
ROW_NUMBER() OVER (ORDER BY a.CTime DESC) AS RowNum
FROM dbo.OM_Order a WITH(NOLOCK)
{where.ToString()}
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
SELECT COUNT(*) FROM dbo.OM_Order a WITH(NOLOCK) {where.ToString()}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var orders = await Connection.QueryAsync<OrderListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<OrderListDto>(count, orders.ToList());
            }
            else
            {
                var orders = await Connection.QueryAsync<OrderListDto>(sql, input, Transaction);

                return new PagedResultDto<OrderListDto>(orders.Count(), orders.ToList());
            }
        }

        public async Task<DataTable> StatOrderByCustomerAsync(StatOrderByCustomerInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("a.CTime>=@StartCTime");
            where.AppendWhere("a.CTime<=@EndCTime");
            where.AppendWhere("a.CustomerID IS NOT NULL");
            where.AppendWhere("b.Name IS NOT NULL");

            string sql = $@"
SELECT
ISNULL(c.Name,'未设置') AS 团体客户类型,
b.Name AS 团体客户,
SUM(a.TotalNum) AS 数量
FROM dbo.OM_Order a WITH(NOLOCK)
LEFT JOIN dbo.MM_Member b WITH(NOLOCK) ON b.ID=a.CustomerID
LEFT JOIN dbo.CM_CustomerType c ON c.ID=b.MemberTypeID
{where}
GROUP BY c.Name,b.Name
";
            var reader = await Connection.ExecuteReaderAsync(sql, input, Transaction);
            var dataTable = new DataTable();
            dataTable.Load(reader);

            return dataTable;
        }

        public async Task<PagedResultDto<SelfHelpTicketGroundListDto>> GetSelfHelpTicketGroundAsync(SelfHelpTicketGroundInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("c.Name IS NOT NULL");
            where.AppendWhereIf(!input.TicketCode.IsNullOrEmpty(), "a.TicketCode = @TicketCode");

            string sql = $@"
SELECT 
 c.Name AS GroundName
,a.TotalNum
,a.SurplusNum 
,d.LastInCheckTime
,a.Etime
,ROW_NUMBER() OVER (ORDER BY a.CTime DESC) AS RowNum
FROM  TM_TicketSale a
LEFT JOIN TM_TicketGroundCache d ON d.TicketCode = a.TicketCode
LEFT JOIN VM_Ground c ON c.Id = d.GroundId
{where.ToString()}
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
SELECT COUNT(*) 
FROM  TM_TicketSale a
LEFT JOIN TM_TicketGroundCache d ON d.TicketCode = a.TicketCode
LEFT JOIN VM_Ground c ON c.Id = d.GroundId
{where.ToString()}
";
            if (input.ShouldPage)
            {
                int count = await Connection.ExecuteScalarAsync<int>(countSql, input, Transaction);
                var orders = await Connection.QueryAsync<SelfHelpTicketGroundListDto>(pagedSql, input, Transaction);

                return new PagedResultDto<SelfHelpTicketGroundListDto>(count, orders.ToList());
            }
            else
            {
                var orders = await Connection.QueryAsync<SelfHelpTicketGroundListDto>(sql, input, Transaction);

                return new PagedResultDto<SelfHelpTicketGroundListDto>(orders.Count(), orders.ToList());
            }
        }
    }
}
