using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTouristByAgeRangeInput
    {
        public DateTime? StartCTime { get; set; }

        [EndTime]
        public DateTime? EndCTime { get; set; }
        public int StatType { get; set; }
    }
}
