using Egoal.Domain.Repositories;
using Egoal.Tickets.Dto;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public interface ITicketSaleBuyerRepository : IRepository<TicketSaleBuyer, long>
    {
        Task<DataTable> StatTouristByAgeRangeAsync(StatTouristByAgeRangeInput input);
        Task<IEnumerable<string>> GetAgeRangesAsync();
        Task<DataTable> StatTouristByAgeRangeSimpleAsync(StatTouristByAgeRangeInput input);
        Task<DataTable> StatTouristByAreaAsync(StatTouristByAreaInput input);
        Task<DataTable> StatTouristBySexAsync(StatTouristBySexInput input);
        Task<List<StatTouristListDto>> StatTouristAsync(StatTouristInput input);
    }
}
