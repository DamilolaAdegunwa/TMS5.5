using Egoal.Domain.Entities;
using System;

namespace Egoal.Tickets
{
    public class TicketGroundType : Entity<long>
    {
        public int GroundTypeId { get; set; }
        public string CardNo { get; set; }
        public string CertNo { get; set; }
        public long? TicketId { get; set; }
        public Guid? TradeId { get; set; }
        public DateTime? Stime { get; set; }
        public DateTime? Etime { get; set; }
        public int? TotalNum { get; set; }
        public int? SurplusNum { get; set; }
        public DateTime? Ctime { get; set; }
        public bool? CommitFlag { get; set; } = true;
        public int? ParkId { get; set; }
        public Guid? SyncCode { get; set; } = Guid.NewGuid();

        public virtual TicketSale TicketSale { get; set; }
    }
}
