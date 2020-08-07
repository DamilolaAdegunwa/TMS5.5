using Egoal.Authorization;
using Egoal.Mvc.Authorization;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Tickets
{
    public class RefundTicketController : TmsControllerBase
    {
        private readonly RefundTicketAppService _refundTicketAppService;

        public RefundTicketController(RefundTicketAppService refundTicketAppService)
        {
            _refundTicketAppService = refundTicketAppService;
        }

        [Route("/Api/Ticket/RefundFromHandsetAsync")]
        [HttpPost]
        [PermissionFilter(Permissions.Handset_RefundTicket)]
        public async Task RefundFromHandsetAsync(RefundInput input)
        {
            await _refundTicketAppService.RefundAsync(input, RefundChannel.安卓手持机);
        }
    }
}
