using Egoal.Domain.Entities;
using System;

namespace Egoal.ValueCards
{
    public class CzkDetail : Entity<long>
    {
        public Guid? MemberAccountId { get; set; }
        public AccountType AccountTypeId { get; set; }
        public int? CzkOpTypeId { get; set; }
        public string CzkOpTypeName { get; set; }
        public Guid? TradeId { get; set; }
        public string ListNo { get; set; }
        public long? CzkId { get; set; }
        public string TicketCode { get; set; }
        public string CardNo { get; set; }
        public int? TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public int? CzkRechargeTypeId { get; set; }
        public string CzkRechargeTypeName { get; set; }
        public int? CzkConsumeTypeId { get; set; }
        public string CzkConsumeTypeName { get; set; }
        public int? CzkCztcId { get; set; }
        public string CzkCztcName { get; set; }
        public decimal? OldCardMoney { get; set; }
        public decimal? OldFreeMoney { get; set; }
        public decimal? OldGameMoney { get; set; }
        public decimal? OldTotalMoney { get; set; }
        public decimal? RechargeCardMoney { get; set; }
        public decimal? RechargeFreeMoney { get; set; }
        public decimal? RechargeGameMoney { get; set; }
        public decimal? RechargeTotalMoney { get; set; }
        public decimal? ConsumeCardMoney { get; set; }
        public decimal? ConsumeFreeMoney { get; set; }
        public decimal? ConsumeGameMoney { get; set; }
        public decimal? ConsumeTotalMoney { get; set; }
        public decimal? ReturnCardMoney { get; set; }
        public decimal? ReturnFreeMoney { get; set; }
        public decimal? ReturnGameMoney { get; set; }
        public decimal? NewCardMoney { get; set; }
        public decimal? NewFreeMoney { get; set; }
        public decimal? NewGameMoney { get; set; }
        public decimal? NewTotalMoney { get; set; }
        public decimal? ReturnMoney { get; set; }
        public decimal? GuiderSharingMoney { get; set; }
        public decimal? UseCouponNum { get; set; }
        public bool? OverdrawFlag { get; set; }
        public decimal? RealPrice { get; set; }
        public decimal? YaJin { get; set; }
        public bool JsFlag { get; set; }
        public string OldEtime { get; set; }
        public string NewEtime { get; set; }
        public int? CashierId { get; set; }
        public string CashierName { get; set; }
        public int? CashPcid { get; set; }
        public string CashPcname { get; set; }
        public int? SalePointId { get; set; }
        public string SalePointName { get; set; }
        public int? SalesmanId { get; set; }
        public string SalesmanName { get; set; }
        public Guid? MemberId { get; set; }
        public string MemberName { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? GuiderId { get; set; }
        public string MemberTel { get; set; }
        public string Memo { get; set; }
        public bool? StatFlag { get; set; }
        public bool? BdFlag { get; set; }
        public Guid? SaleSyncCode { get; set; }
        public Guid? TicketCheckSyncCode { get; set; }
        public int? TradeNum { get; set; }
        public long? ReturnCzkDetailId { get; set; }
        public string Cdate { get; set; }
        public string Cweek { get; set; }
        public string Cmonth { get; set; }
        public string Cquarter { get; set; }
        public string Cyear { get; set; }
        public string Ctp { get; set; }
        public DateTime? Ctime { get; set; }
        public DateTime? Ltime { get; set; }
        public long? Bid { get; set; }
        public bool? StatBalanceFlag { get; set; }
        public bool? CommitFlag { get; set; } = true;
        public int? ParkId { get; set; }
        public string ParkName { get; set; }
        public Guid SyncCode { get; set; } = Guid.NewGuid();
    }
}
