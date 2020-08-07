using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTicketSaleByTicketTypeClassInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public int? TicketTypeId { get; set; }
        public int? TicketTypeClassId { get; set; }
    }
}
