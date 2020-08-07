using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Tickets.Dto
{
    public class StatCzkSaleInput
    {
        public DateTime? StartCtime { get; set; }

        public DateTime? EndCtime { get; set; }

        public int? TicketTypeId { get; set; }

        public int? CashierId { get; set; }
    }
}
