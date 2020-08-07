using Egoal.Application.Services.Dto;
using Egoal.ValueCards.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.ValueCards
{
    public interface IValueCardQueryAppService
    {
        Task<byte[]> QueryCzkDetailsToExcelAsync(QueryCzkDetailInput input);
        Task<PagedResultDto<CzkDetailListDto>> QueryCzkDetailsAsync(QueryCzkDetailInput input);
        Task<List<ComboboxItemDto<int>>> GetCzkCztcComboboxItemsAsync();
    }
}
