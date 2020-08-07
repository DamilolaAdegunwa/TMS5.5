using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Customers
{
    public class PromoterAppService : ApplicationService, IPromoterAppService
    {
        private readonly IRepository<Promoter> _promoterRepository;

        public PromoterAppService(IRepository<Promoter> promoterRepository)
        {
            _promoterRepository = promoterRepository;
        }

        public async Task<List<ComboboxItemDto<int>>> GetPromoterComboboxItemsAsync()
        {
            var query = _promoterRepository.GetAll()
                .Where(e => e.IsApproved)
                .OrderBy(e => e.Id)
                .Select(c => new ComboboxItemDto<int>
                {
                    DisplayText = c.Name,
                    Value = c.Id
                });

            return await _promoterRepository.ToListAsync(query);
        }
    }
}
