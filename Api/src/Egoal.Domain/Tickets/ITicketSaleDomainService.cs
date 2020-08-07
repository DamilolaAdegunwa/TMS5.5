using Egoal.Scenics.Dto;
using Egoal.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public interface ITicketSaleDomainService
    {
        Task<bool> ShouldInValidAsync(TicketSale ticketSale, int surplusQuantity, int refundQuantity);
        Task<int> GetConsumeNumAsync(TicketSale ticketSale);
        Task<int> GetRealNumAsync(TicketSale ticketSale);
        Task<int> GetRefundNumAsync(TicketSale ticketSale);
        Task<int> GetSurplusNumAsync(TicketSale ticketSale);
        Task<int> GetPhotoQuantityAsync(TicketSale ticketSale);
        Task<DateTime?> GetLastCheckInTimeAsync(long id, bool isChecking = false);
        Task<bool> IsUsableAsync(TicketSale ticketSale);
        Task<bool> AllowRefundAsync(TicketSale ticketSale);
        Task<bool> AllowEnrollFaceAsync(TicketSale ticketSale);
        Task ActiveAsync(TicketSale ticketSale, IEnumerable<GroundChangCiDto> groundChangCis = null);
        Task<TicketSale> RenewAsync(long ticketId);
        Task InValidAsync(TicketSale ticketSale);
        Task<int> ConsumeAsync(TicketSale ticketSale, ConsumeTicketInput input);
        Task CheckOutAsync(TicketSale ticketSale, CheckOutTicketInput input);
    }
}
