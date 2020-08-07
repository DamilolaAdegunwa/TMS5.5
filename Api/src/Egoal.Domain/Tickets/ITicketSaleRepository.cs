using Egoal.Application.Services.Dto;
using Egoal.Domain.Repositories;
using Egoal.Tickets.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public interface ITicketSaleRepository : IRepository<TicketSale, long>
    {
        Task InValidAsync(TicketSale ticketSale);
        Task RefundAsync(TicketSale ticketSale, int quantity);
        Task<int> GetFingerprintQuantityAsync(long ticketId);
        Task<DateTime> GetFacePhotoBindTimeAsync(long ticketId);
        Task<PagedResultDto<TicketSale>> GetTicketsByMemberAsync(GetTicketsByMemberInput input);
        Task<PagedResultDto<TicketSaleListDto>> QueryTicketSalesAsync(QueryTicketSaleInput input);
        Task<List<StatTicketSaleListDto>> StatAsync(StatTicketSaleInput input);
        Task<DataTable> StatCashierSaleAsync(StatCashierSaleInput input);
        Task<DataTable> StatPromoterSaleAsync(StatPromoterSaleInput input);
        Task<DataTable> StatByTradeSourceAsync(StatTicketSaleByTradeSourceInput input);
        Task<DataTable> StatByTicketTypeClassAsync(StatTicketSaleByTicketTypeClassInput input);
        Task<DataTable> StatByPayTypeAsync(StatTicketSaleByPayTypeInput input, IEnumerable<ComboboxItemDto<int>> payTypes);
        Task<DataTable> StatBySalePointAsync(StatTicketSaleBySalePointInput input);
        Task<DataTable> StatGroundSharingAsync(StatGroundSharingInput input);
        Task<DataTable> StatJbAsync(StatJbInput input);
        Task<DataTable> StatByCustomerAsync(StatTicketSaleByCustomerInput input);
        Task<DataTable> StatCzkSaleAsync(StatCzkSaleInput input);
        Task<DataTable> StatCzkSaleJbAsync(StatJbInput input);
    }
}
