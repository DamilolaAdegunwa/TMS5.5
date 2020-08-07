using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.TicketTypes.Dto
{
    public class TicketTypeForSelfHelpDto : TicketTypeForSaleListDto
    {
        public bool NeedCertFlag { get; set; }

        public int? MinBuyNum { get; set; }

        public int? MaxBuyNum { get; set; }
    }
}
