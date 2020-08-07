using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets.Dto
{
    public class StatTouristNumInput
    {
        public DateTime? SSDate { get; set; }

        public DateTime? ESDate { get; set; }

        public int? GateGroupId { get; set; }

        public int? GateId { get; set; }
    }
}
