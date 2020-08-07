using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class QueryExchangeHistoryInput : PagedInputDto
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public string OrderListNo { get; set; }
        public string OldTicketCode { get; set; }
        public int? TicketTypeId { get; set; }
        public int? CashierId { get; set; }
        public int? SalePointId { get; set; }
    }
}
