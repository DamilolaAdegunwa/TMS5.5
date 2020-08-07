using Egoal.Domain.Entities;
using System;

namespace Egoal.Tickets
{
    public class TicketGroundSale : Entity<long>
    {
        public Guid? TradeId { get; set; }
        public string ListNo { get; set; }
        public long? TicketId { get; set; }
        public int? GroundId { get; set; }
        public int? PersonNum { get; set; }
        public int? UsedPersonNum { get; set; }
        public int? UnusedPersonNum { get; set; }
        public decimal? RealPrice { get; set; }
        public decimal? RealMoney { get; set; }
        public int? CashierId { get; set; }
        public int? CashPcId { get; set; }
        public int? SalePointId { get; set; }
        public DateTime? CTime { get; set; } = DateTime.Now;

        public virtual TicketSale TicketSale { get; set; }
    }
}
