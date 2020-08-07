using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers.Tickets
{
    public class PrintTicketController : TmsControllerBase
    {
        private readonly PrintTicketAppService _printTicketAppService;

        public PrintTicketController(PrintTicketAppService printTicketAppService)
        {
            _printTicketAppService = printTicketAppService;
        }

        [Route("/Api/Ticket/RePrintAsync")]
        [HttpPost]
        public async Task RePrintAsync(PrintTicketInput input)
        {
            await _printTicketAppService.RePrintAsync(input);
        }

        [Route("/Api/Ticket/PrintAsync")]
        [HttpPost]
        public async Task PrintAsync(PrintTicketInput input)
        {
            await _printTicketAppService.PrintAsync(input);
        }
    }
}
