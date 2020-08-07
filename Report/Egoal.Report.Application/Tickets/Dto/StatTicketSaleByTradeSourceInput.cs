using System;

namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketSaleByTradeSourceInput
    {
        public string StartCTime { get; set; }
        public string EndCTime { get; set; }
        public int? TicketTypeId { get; set; }
        public int? TradeSource { get; set; }
    }
}
