using Egoal.Authorization;
using Egoal.Mvc.Authorization;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Tickets
{
    public class ConsumeTicketController : TmsControllerBase
    {
        private readonly ConsumeTicketAppService _consumeTicketAppService;

        public ConsumeTicketController(ConsumeTicketAppService consumeTicketAppService)
        {
            _consumeTicketAppService = consumeTicketAppService;
        }

        [Route("/Api/Ticket/CheckTicketFromHandsetAsync")]
        [HttpPost]
        [PermissionFilter(Permissions.Handset_CheckTicket)]
        public async Task<JsonResult> CheckTicketFromHandsetAsync(CheckTicketInput input)
        {
            input.ConsumeType = ConsumeType.安卓手持机检票;

            var result = await _consumeTicketAppService.CheckTicketAsync(input);

            return Json(result);
        }

        [Route("/Api/Ticket/CheckTicketFromMobileAsync")]
        [HttpPost]
        [PermissionFilter(Permissions.TMSWeChat_CheckTicket)]
        public async Task<JsonResult> CheckTicketFromMobileAsync(CheckTicketInput input)
        {
            input.ConsumeType = ConsumeType.手机检票;

            var result = await _consumeTicketAppService.CheckTicketAsync(input);

            return Json(result);
        }
    }
}
