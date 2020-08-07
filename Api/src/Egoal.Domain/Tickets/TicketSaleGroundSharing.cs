using Egoal.Domain.Entities;
using System;

namespace Egoal.Tickets
{
    public class TicketSaleGroundSharing : Entity<long>
    {
        public long? TicketId { get; set; }
        public int? GroundId { get; set; }
        public decimal? SharingRate { get; set; }
        public decimal? SharingPrice { get; set; }
        public int? SharingNum { get; set; }
        public decimal? SharingMoney { get; set; }
        public DateTime? CTime { get; set; } = DateTime.Now;

        public virtual TicketSale TicketSale { get; set; }
    }
}
