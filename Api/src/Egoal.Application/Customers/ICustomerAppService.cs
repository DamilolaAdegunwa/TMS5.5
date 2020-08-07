using Egoal.Application.Services.Dto;
using Egoal.Customers.Dto;
using Egoal.Members.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Customers
{
    public interface ICustomerAppService
    {
        Task RegistFromMobileAsync(MobileRegistInput input);
        Task CreateAsync(EditDto input);
        Task<EditDto> GetForEditAsync(Guid id);
        Task EditAsync(EditDto input);
        Task AuditAsync(AuditInput input);
        Task ChangePasswordAsync(ChangePasswordInput input);
        Task UnBindMemberAsync(UnBindMemberInput input);
        Task DeleteAsync(Guid id);
        Task<LoginOutput> LoginFromWeChatAsync(CustomerLoginInput input);
        Task<PagedResultDto<CustomerListDto>> GetCustomersAsync(GetCustomersInput input);
        Task<List<ComboboxItemDto<int>>> GetCustomerTypeComboboxItemsAsync();
        Task<List<ComboboxItemDto>> GetCustomerComboboxItemsAsync();
        Task<List<ComboboxItemDto>> GetConsumeCustomerComboBoxItemsAsync();
    }
}
