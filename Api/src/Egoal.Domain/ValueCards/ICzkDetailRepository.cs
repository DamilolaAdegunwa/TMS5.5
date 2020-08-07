using Egoal.Application.Services.Dto;
using Egoal.Domain.Repositories;
using Egoal.ValueCards.Dto;
using System.Threading.Tasks;

namespace Egoal.ValueCards
{
    public interface ICzkDetailRepository : IRepository<CzkDetail, long>
    {
        Task<PagedResultDto<CzkDetailListDto>> QueryCzkDetailsAsync(QueryCzkDetailInput input);
    }
}
