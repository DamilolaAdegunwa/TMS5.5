using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Payment
{
    public class PayTypeAppService : ApplicationService, IPayTypeAppService
    {
        private readonly IRepository<PayType> _payTypeRepository;

        public PayTypeAppService(IRepository<PayType> payTypeRepository)
        {
            _payTypeRepository = payTypeRepository;
        }

        public async Task<List<ComboboxItemDto<int>>> GetPayTypeComboboxItemsAsync()
        {
            var query = _payTypeRepository.GetAll()
                .OrderBy(p => p.SortCode)
                .Select(p => new ComboboxItemDto<int>
                {
                    Value = p.Id,
                    DisplayText = p.Name
                });

            return await _payTypeRepository.ToListAsync(query);
        }
    }
}
