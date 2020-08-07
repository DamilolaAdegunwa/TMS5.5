using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.Excel;
using Egoal.ValueCards.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.ValueCards
{
    public class ValueCardQueryAppService : ApplicationService, IValueCardQueryAppService
    {
        private readonly INameCacheService _nameCacheService;
        private readonly ICzkDetailRepository _czkDetailRepository;
        private readonly IRepository<CzkCztc> _czkCztcRepository;

        public ValueCardQueryAppService(
            INameCacheService nameCacheService,
            ICzkDetailRepository czkDetailRepository,
            IRepository<CzkCztc> czkCztcRepository)
        {
            _nameCacheService = nameCacheService;
            _czkDetailRepository = czkDetailRepository;
            _czkCztcRepository = czkCztcRepository;
        }

        public async Task<byte[]> QueryCzkDetailsToExcelAsync(QueryCzkDetailInput input)
        {
            input.ShouldPage = false;

            var result = await QueryCzkDetailsAsync(input);

            return await ExcelHelper.ExportToExcelAsync(result.Items, "储值卡明细查询", string.Empty);
        }

        public async Task<PagedResultDto<CzkDetailListDto>> QueryCzkDetailsAsync(QueryCzkDetailInput input)
        {
            var result = await _czkDetailRepository.QueryCzkDetailsAsync(input);

            foreach (var item in result.Items)
            {
                item.CzkOpTypeName = item.CzkOpTypeId?.ToString();
                item.TicketTypeName = _nameCacheService.GetTicketTypeName(item.TicketTypeId);
                item.CzkRechargeTypeName = item.CzkRechargeTypeId?.ToString();
                item.CzkCztcName = _nameCacheService.GetCzkCztcName(item.CzkCztcId);
                item.CzkConsumeTypeName = item.CzkConsumeTypeId?.ToString();
                item.MemberName = _nameCacheService.GetMemberName(item.MemberId);
                item.PayTypeName = _nameCacheService.GetPayTypeName(item.PayTypeId);
                if (item.CashierId.HasValue)
                {
                    item.CashierName = _nameCacheService.GetStaffName(item.CashierId);
                }
                else if (item.GroundId.HasValue)
                {
                    item.CashierName = _nameCacheService.GetGroundName(item.GroundId);
                }
            }

            return result;
        }

        public async Task<List<ComboboxItemDto<int>>> GetCzkCztcComboboxItemsAsync()
        {
            var query = _czkCztcRepository.GetAll()
                .OrderBy(t => t.SortCode)
                .Select(t => new ComboboxItemDto<int>
                {
                    Value = t.Id,
                    DisplayText = t.Name
                });

            var items = await _czkCztcRepository.ToListAsync(query);

            return items;
        }
    }
}
