using Egoal.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Common
{
    public interface ICommonAppService
    {
        List<ComboboxItemDto> GetEducationComboboxItems();
        List<ComboboxItemDto> GetNationComboboxItems();
        Task<List<ComboboxItemDto<int>>> GetAgeRangeComboboxItemsAsync();
        Task<TreeNodeDto> GetTouristOriginTreeAsync();
        Task<List<ComboboxItemDto<int>>> GetCertTypeComboboxItemsAsync();
        Task SendVerificationCodeAsync(string address);
        void ValidateVerificationCode(string address, string code);
    }
}
