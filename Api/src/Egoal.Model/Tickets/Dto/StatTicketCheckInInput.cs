using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTicketCheckInInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public int? ParkId { get; set; }
        public int? GateGroupId { get; set; }
        public int? GateId { get; set; }
        public int? CheckerId { get; set; }
        public int StatType { get; set; }

        public int? GroundId { get; set; }

        public bool IfByGround { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
