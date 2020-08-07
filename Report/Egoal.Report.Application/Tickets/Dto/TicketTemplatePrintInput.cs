using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Tickets.Dto
{
    public class TicketTemplatePrintInput
    {
        public string CompanyName { get; set; }

        public string TicketTypeName { get; set; }

        public string ETime { get; set; }

        public string PersonNum { get; set; }

        public string TicketCode { get; set; }

        public string CTime { get; set; }

        public string DistributorName { get; set; }

        public string ReaMoney { get; set; }

        public string SalePointName { get; set; }

        public string ChangCi { get; set; }

        public string Seat { get; set; }
    }
}
