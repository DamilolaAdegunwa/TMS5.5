using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Tickets.Dto
{
    public class StatTouristNumInput
    {
        public DateTime? SSDate { get; set; }

        public DateTime? ESDate { get; set; }

        public int? GateGroupId { get; set; }

        public int? GateId { get; set; }
    }
}
