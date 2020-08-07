using Egoal.Application.Services.Dto;
using System;

namespace Egoal.ValueCards.Dto
{
    public class QueryCzkDetailInput : PagedInputDto
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public int? CzkOpTypeId { get; set; }
        public int? CzkRechargeTypeId { get; set; }
        public int? CzkCztcId { get; set; }
        public int? CzkConsumeTypeId { get; set; }
        public int? TicketTypeId { get; set; }
        public int? CashierId { get; set; }
        public Guid? MemberId { get; set; }
        public string ListNo { get; set; }
        public string CardNo { get; set; }
        public int? PayTypeId { get; set; }
    }
}
