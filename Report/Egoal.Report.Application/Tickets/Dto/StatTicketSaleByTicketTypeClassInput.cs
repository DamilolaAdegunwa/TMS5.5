using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketSaleByTicketTypeClassInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? TicketTypeId { get; set; }
        public int? TicketTypeClassId { get; set; }
    }
}
