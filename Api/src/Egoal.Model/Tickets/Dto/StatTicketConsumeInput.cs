using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTicketConsumeInput
    {
        public DateTime? StartCheckTime { get; set; }

        [EndTime]
        public DateTime? EndCheckTime { get; set; }
        public DateTime? StartConsumeTime { get; set; }

        [EndTime]
        public DateTime? EndConsumeTime { get; set; }
        public int? TicketTypeId { get; set; }
        public string CustomerId { get; set; }
        public ConsumeType? ConsumeType { get; set; }
    }
}
