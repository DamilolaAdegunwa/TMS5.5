using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets.Dto
{
    public class StatCzkSaleInput
    {
        public DateTime? StartCtime { get; set; }

        public DateTime? EndCtime { get; set; }

        public int? TicketTypeId { get; set; }

        public int? CashierId { get; set; }
    }
}
