using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class QueryTicketCheckInput : PagedInputDto
    {
        public long? TicketId { get; set; }
        public DateTime? StartCheckTime { get; set; }

        [EndTime]
        public DateTime? EndCheckTime { get; set; }
        public int? GroundId { get; set; }
        public int? GateGroupId { get; set; }
        public int? GateId { get; set; }
        public string ListNo { get; set; }
        public string TicketCode { get; set; }
        public string CardNo { get; set; }
        public int? TicketTypeId { get; set; }
        public int? CashierId { get; set; }
        public int? CashPcid { get; set; }
        public int? ParkId { get; set; }
        public bool ShouldSum { get; set; } = true;
    }
}
