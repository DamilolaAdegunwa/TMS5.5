using Egoal.Domain.Entities;
using System;

namespace Egoal.ValueCards
{
    public class MemberAccount : Entity<Guid>
    {
        public AccountType AccountTypeId { get; set; }
        public string Name { get; set; }
        public Guid? MemberId { get; set; }
        public Guid? TradeId { get; set; }
        public string ListNo { get; set; }
        public string Pwd { get; set; }
        public string Salt { get; set; }
        public bool? ActiveFlag { get; set; }
        public string ActiveFlagName { get; set; }
        public int? TicketStatusId { get; set; }
        public string TicketStatusName { get; set; }
        public decimal? PrincipalBalance { get; set; }
        public decimal? FreeMoney { get; set; }
        public decimal? GameMoney { get; set; }
        public decimal? TotalMoney { get; set; }
        public int? PrincipalNum { get; set; }
        public int? FreeNum { get; set; }
        public int? TotalNum { get; set; }
        public long? Point { get; set; }
        public bool? OverdrawFlag { get; set; }
        public decimal? OverdrawnAmount { get; set; }
        public decimal? WarnMoney { get; set; }
        public int? CashierId { get; set; }
        public string CashierName { get; set; }
        public int? CashPcid { get; set; }
        public string CashPcname { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? GuiderId { get; set; }
        public int? SalePointId { get; set; }
        public string SalePointName { get; set; }
        public string Memo { get; set; }
        public DateTime? Ctime { get; set; }
        public DateTime? Ltime { get; set; }
        public long? Bid { get; set; }
        public bool? CommitFlag { get; set; } = true;
        public int? ParkId { get; set; }
        public string ParkName { get; set; }
        public Guid SyncCode { get; set; } = Guid.NewGuid();
    }
}
