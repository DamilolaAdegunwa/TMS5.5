using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Trades.Dto
{
    public class StatPayDetailByCustomerInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public Guid? CustomerId { get; set; }
        public TradeSource? TradeSource { get; set; }
        public int? PayTypeId { get; set; }
    }
}
