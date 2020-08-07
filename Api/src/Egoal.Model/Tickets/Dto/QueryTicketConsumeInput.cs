using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class QueryTicketConsumeInput : PagedInputDto
    {
        public DateTime? StartCheckTime { get; set; }

        [EndTime]
        public DateTime? EndCheckTime { get; set; }
        public DateTime? StartConsumeTime { get; set; }

        [EndTime]
        public DateTime? EndConsumeTime { get; set; }
        public DateTime? StartTravelDate { get; set; }
        public DateTime? EndTravelDate { get; set; }
        public string ListNo { get; set; }
        public string TicketCode { get; set; }
        public int? TicketTypeId { get; set; }
        public string CustomerId { get; set; }
        public ConsumeType? ConsumeType { get; set; }
        public bool? HasNoticed { get; set; }
    }
}
