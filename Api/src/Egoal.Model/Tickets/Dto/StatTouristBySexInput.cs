using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTouristBySexInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
    }
}
