using Egoal.Application.Services.Dto;
using Egoal.Domain.Repositories;
using Egoal.Orders.Dto;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public interface IOrderRepository : IRepository<Order, string>
    {
        Task<Order> GetByIdAsync(string id);
        Task<int> GetMemberBuyQuantityAsync(Guid memberId, int ticketTypeId, DateTime startTime, DateTime endTime);
        Task<int> GetCertBuyQuantityAsync(string certNo, DateTime startTime, DateTime endTime);
        Task<bool> HasExchangedAsync(string listNo);
        Task<DateTime?> GetOrderCheckInTimeAsync(string listNo);
        Task<DateTime?> GetOrderCheckOutTimeAsync(string listNo);
        Task<PagedResultDto<OrderListDto>> GetOrdersAsync(GetOrdersInput input);
        Task<DataTable> StatOrderByCustomerAsync(StatOrderByCustomerInput input);
        Task<PagedResultDto<SelfHelpTicketGroundListDto>> GetSelfHelpTicketGroundAsync(SelfHelpTicketGroundInput input);
    }
}
