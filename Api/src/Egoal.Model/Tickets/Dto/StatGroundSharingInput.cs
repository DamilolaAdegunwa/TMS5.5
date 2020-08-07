using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatGroundSharingInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public int? SalePointId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? GroundId { get; set; }
    }
}
