using Egoal.Application.Services.Dto;
using Egoal.Trades;
using System;

namespace Egoal.Tickets.Dto
{
    public class GetTicketsByMemberInput : PagedInputDto
    {
        public Guid MemberId { get; set; }
        public Guid? CustomerId { get; set; }
        public TradeSource TradeSource { get; set; }
        public DateTime ETime { get; set; }
    }
}
