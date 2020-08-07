using System;

namespace Egoal.Report.Tickets.Dto
{
    public class StatTicketSaleByCustomerInput
    {
        public DateTime? StartCTime { get; set; }
        public DateTime? EndCTime { get; set; }
        public DateTime? StartTravelDate { get; set; }
        public DateTime? EndTravelDate { get; set; }
        public Guid? CustomerId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? TradeSource { get; set; }
    }
}
