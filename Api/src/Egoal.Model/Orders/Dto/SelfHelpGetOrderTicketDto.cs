using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Orders.Dto
{
    public class SelfHelpGetOrderTicketDto
    {
        public SelfHelpGetOrderTicketDto()
        {
            SelfHelpGetTickets = new List<SelfHelpGetTicket>();
        }

        public List<SelfHelpGetTicket> SelfHelpGetTickets { get; set; }
    }

    public class SelfHelpGetTicket
    {
        public int TicketTypeId { get; set; }

        public string TicketTypeName { get; set; }

        public string Etime { get; set; }

        public int PersonNum { get; set; }

        public string TicketCode { get; set; }

        public string CompanyName { get; set; }

        public string Ctime { get; set; }

        public string DistributorName { get; set; }

        public decimal ReaMoney { get; set; }

        public string SalePointName { get; set; }

        public string ChangeCiName { get; set; }

        public string SeatName { get; set; }

        public int RowNum { get; set; }
    }
}
