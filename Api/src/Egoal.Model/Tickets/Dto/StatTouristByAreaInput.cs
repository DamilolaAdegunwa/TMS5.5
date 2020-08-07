using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTouristByAreaInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public string ProvinceName { get; set; }
        public int StatType { get; set; }
    }
}
