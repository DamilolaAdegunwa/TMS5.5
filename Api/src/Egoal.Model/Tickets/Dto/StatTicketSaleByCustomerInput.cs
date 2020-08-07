using Egoal.Application.Services.Dto;
using Egoal.Trades;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatTicketSaleByCustomerInput
    {
        public DateTime? StartCTime { get; set; }

        [EndTime]
        public DateTime? EndCTime { get; set; }
        public DateTime? StartTravelDate { get; set; }
        public DateTime? EndTravelDate { get; set; }
        public Guid? CustomerId { get; set; }
        public int? TicketTypeId { get; set; }
        public TradeSource? TradeSource { get; set; }
    }
}
