using Egoal.Domain.Entities;
using System;

namespace Egoal.ValueCards
{
    public class TicketBlacklistCheck : Entity<long>
    {
        public Guid? TradeId { get; set; }
        public long? TicketId { get; set; }
        public string TicketCode { get; set; }
        public int CheckTypeId { get; set; }
        public string CardNo { get; set; }
        public int TotalNum { get; set; }
        public int CorrectNum { get; set; }
        public bool InBlackList { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public bool? CommitFlag { get; set; } = true;
        public int? ParkId { get; set; }
        public Guid? SyncCode { get; set; } = Guid.NewGuid();
    }
}
