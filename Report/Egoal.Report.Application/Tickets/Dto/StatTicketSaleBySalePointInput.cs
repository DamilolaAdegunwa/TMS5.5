using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketSaleBySalePointInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? ParkId { get; set; }
        public int? SalePointId { get; set; }
        public int? TicketTypeId { get; set; }
    }
}
