using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Tickets.Dto
{
    public class StatPromoterSaleInput
    {
        public DateTime StartCTime { get; set; }

        [EndTime]
        public DateTime EndCTime { get; set; }
        public int? PromoterId { get; set; }
        public bool IncludeGroundRefund { get; set; }
    }
}
