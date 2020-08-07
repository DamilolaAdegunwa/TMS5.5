using System;

namespace Egoal.Report.Tickets.Dto
{
    public class StatPromoterSaleInput
    {
        public DateTime StartCTime { get; set; }
        public DateTime EndCTime { get; set; }
        public int? PromoterId { get; set; }
    }
}
