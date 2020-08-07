using Egoal.Application.Services.Dto;
using Egoal.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    public class PromoterController : TmsControllerBase
    {
        private readonly IPromoterAppService _promoterAppService;

        public PromoterController(IPromoterAppService promoterAppService)
        {
            _promoterAppService = promoterAppService;
        }

        [HttpGet]
        public async Task<List<ComboboxItemDto<int>>> GetPromoterComboboxItemsAsync()
        {
            return await _promoterAppService.GetPromoterComboboxItemsAsync();
        }
    }
}
