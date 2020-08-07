using Egoal.Application.Services.Dto;
using Egoal.Scenics.Dto;
using Egoal.TicketTypes.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.TicketTypes
{
    public interface ITicketTypeQueryAppService
    {
        Task<List<TicketTypeForSaleListDto>> GetTicketTypesForSaleAsync(GetTicketTypesForSaleInput input);
        Task<List<TicketTypeForSelfHelpDto>> GetTicketTypesForSelfHelpAsync(GetTicketTypesForSaleInput input);
        Task<PagedResultDto<TicketTypeDescriptionDto>> GetTicketTypeDescriptionsAsync(GetTicketTypeDescriptionsInput input);
        Task<TicketTypeDescriptionDto> GetTicketTypeDescriptionAsync(int ticketTypeId);
        Task<TicketTypeForNetSaleDto> GetTicketTypeForNetSaleAsync(int ticketTypeId, SaleChannel saleChannel, Guid memberId);
        Task<List<GroundChangCisDto>> GetTicketTypeChangCiComboboxItemsAsync(int ticketTypeId, DateTime date);
        Task<List<GroundChangCisDto>> GetGroundChangCisDtosVariedAsync(int ticketTypeId, DateTime date);
        Task<List<ComboboxItemDto<int>>> GetTicketTypeTypeComboboxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetTicketTypeComboboxItemsAsync(TicketTypeType? ticketTypeTypeId);
        Task<List<ComboboxItemDto<int>>> GetNetSaleTicketTypeComboboxItemsAsync();
        Task<List<ComboboxItemDto<int>>> GetTicketTypeClassComboboxItemsAsync();
    }
}
