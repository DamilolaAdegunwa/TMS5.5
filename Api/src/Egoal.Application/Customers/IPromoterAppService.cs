using Egoal.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Customers
{
    public interface IPromoterAppService
    {
        Task<List<ComboboxItemDto<int>>> GetPromoterComboboxItemsAsync();
    }
}
