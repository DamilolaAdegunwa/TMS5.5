using Dapper;
using Egoal.EntityFrameworkCore;
using Egoal.TicketTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Egoal.Extensions;
using Egoal.Application.Services.Dto;
using System.Linq;

namespace Egoal.TicketTypes
{
    public class TicketTypeRepository : EfCoreRepositoryBase<TicketType>, ITicketTypeRepository
    {
        public TicketTypeRepository(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public async Task<IEnumerable<TicketType>> GetTicketTypesForSaleAsync(GetTicketTypesForSaleInput input)
        {
            StringBuilder where = new StringBuilder();
            where.AppendWhere("ID>0");
            where.AppendWhere("SaleFlag=1");
            where.AppendWhere("(TicketTypeTypeID=1 OR TicketTypeTypeID=3)");
            where.AppendWhereIf(input.SaleChannel == SaleChannel.Net, "BookSDate<=@SaleDate");
            where.AppendWhereIf(input.SaleChannel == SaleChannel.Net, "BookEDate>=@SaleDate");
            where.AppendWhere("ValidDate>=@SaleDate");
            where.AppendWhereIf(input.SaleChannel == SaleChannel.Net, "XsTypeID>=2");
            where.AppendWhereIf(input.SaleChannel == SaleChannel.Local, "XsTypeID<=2");
            where.AppendWhereIf(input.PublicSaleFlag.HasValue, "PublicSaleFlag=@PublicSaleFlag");
            where.AppendWhereIf(!input.KeyWord.IsNullOrEmpty(), $"([Name] LIKE '{input.KeyWord}%' OR Zjf LIKE '{input.KeyWord}%' OR Code LIKE '{input.KeyWord}%')");

            string sql = $@"
SELECT 
*
FROM dbo.TM_TicketType
{where}
ORDER BY SortCode
";
            var param = new { input.SaleDate, input.PublicSaleFlag };
            return await Connection.QueryAsync<TicketType>(sql, param, Transaction);
        }

        public async Task<bool> HasSpecifiedCheckGroundAsync(int ticketTypeId)
        {
            string sql = @"
SELECT TOP 1 1 
FROM dbo.TM_TicketTypeGround 
WHERE TicketTypeID=@ticketTypeId
";
            return (await Connection.ExecuteScalarAsync<string>(sql, new { ticketTypeId }, Transaction)) == "1";
        }

        public async Task<bool> HasGrantedToGroundAsync(int ticketTypeId, int groundId)
        {
            string sql = @"
SELECT TOP 1 1 
FROM dbo.TM_TicketTypeGround 
WHERE TicketTypeID=@ticketTypeId
AND GroundID=@groundId
";
            return (await Connection.ExecuteScalarAsync<string>(sql, new { ticketTypeId, groundId }, Transaction)) == "1";
        }

        public async Task<bool> HasGrantedToStaffAsync(int ticketTypeId, int staffId)
        {
            string sql = @"
SELECT TOP 1 1
FROM dbo.TM_TicketTypeGroupDetail a
JOIN dbo.RM_Staff b ON b.TicketTypeGroupID=a.TicketTypeGroupID
WHERE a.TicketTypeID=@ticketTypeId
AND b.ID=@staffId
";
            return (await Connection.ExecuteScalarAsync<string>(sql, new { ticketTypeId, staffId }, Transaction)) == "1";
        }

        public async Task<bool> HasGrantedToSalePointAsync(int ticketTypeId, int salePointId)
        {
            string sql = @"
SELECT TOP 1 1 
FROM dbo.TM_SalePointTicketType 
WHERE TicketTypeID=@ticketTypeId 
AND SalePointID=@salePointId
";
            return (await Connection.ExecuteScalarAsync<string>(sql, new { ticketTypeId, salePointId }, Transaction)) == "1";
        }

        public async Task<bool> HasGrantedToParkAsync(int ticketTypeId, int parkId)
        {
            string sql = @"
SELECT TOP 1 1 
FROM dbo.TM_ParkTicketType 
WHERE TicketTypeID=@ticketTypeId 
AND ParkID=@parkId
";
            return (await Connection.ExecuteScalarAsync<string>(sql, new { ticketTypeId, parkId }, Transaction)) == "1";
        }

        public async Task<List<ComboboxItemDto<int>>> GetTicketTypeTypeComboboxItemsAsync()
        {
            string sql = @"
SELECT
ID AS Value,
Name AS DisplayText
FROM dbo.TM_TicketTypeType
ORDER BY SortCode
";
            return (await Connection.QueryAsync<ComboboxItemDto<int>>(sql, null, Transaction)).ToList();
        }

        public async Task<List<TicketTypeDailyPriceDto>> GetPriceAsync(int ticketTypeId, string startDate, string endDate)
        {
            string sql = @"
SELECT
a.TicketTypeID,
b.Date,
a.TicPrice,
a.NetPrice,
a.PrintPrice
FROM dbo.TM_TicketTypeDateTypePrice a WITH(NOLOCK)
LEFT JOIN dbo.TM_Date b WITH(NOLOCK) ON b.DateTypeID = a.DateTypeID
WHERE a.TicketTypeID=@ticketTypeId
AND b.Date>=@startDate
AND b.Date<=@endDate
";
            return (await Connection.QueryAsync<TicketTypeDailyPriceDto>(sql, new { ticketTypeId, startDate, endDate }, Transaction)).ToList();
        }

        public async Task<List<TicketTypeGroundType>> GetGrantedGroundTypesAsync(int ticketTypeId)
        {
            string sql = @"
SELECT
y.*
FROM dbo.TM_TicketTypeGroundType y
JOIN
(
	SELECT DISTINCT
	a.TicketTypeID,
	b.GroundTypeID
	FROM dbo.TM_TicketTypeGround a
	JOIN dbo.VM_Ground b ON b.ID=a.GroundID
	WHERE a.TicketTypeID=@ticketTypeId
)x ON x.GroundTypeID=y.GroundTypeID AND x.TicketTypeID=y.TicketTypeID
";
            return (await Connection.QueryAsync<TicketTypeGroundType>(sql, new { ticketTypeId }, Transaction)).ToList();
        }
    }
}
