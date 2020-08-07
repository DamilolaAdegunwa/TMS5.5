using Egoal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Wares
{
    public class WareIODetail : Entity<long>
    {
        public Guid? TradeId { get; set; }
        public string ListNo { get; set; }
        public int? TradeTypeId { get; set; }
        public string TradeTypeName { get; set; }
        public Guid? WareId { get; set; }
        public string WareName { get; set; }
        public int? WareHouseId { get; set; }
        public Guid? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Barcode { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RentPrice { get; set; }
        public decimal? YaJin { get; set; }
        public decimal? Ionum { get; set; }
        public decimal? IonumAbs { get; set; }
        public int? DiscountTypeId { get; set; }
        public string DiscountTypeName { get; set; }
        public decimal? DiscountRate { get; set; }
        public int? DiscountApproverId { get; set; }
        public string DiscountApproverName { get; set; }
        public decimal? ReaPrice { get; set; }
        public decimal? ReaMoney { get; set; }
        public decimal? GuiderSharingMoney { get; set; }
        public decimal? CostMoney { get; set; }
        public int? RentMinutes { get; set; }
        public decimal? Profit { get; set; }
        public int? PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public bool? PayFlag { get; set; }
        public bool? CzkFlag { get; set; }
        public long? CzkId { get; set; }
        public Guid? MemberAccountId { get; set; }
        public string CzkTicketCode { get; set; }
        public string CzkCardNo { get; set; }
        public string CzkOwner { get; set; }
        public string CzkOwnerTel { get; set; }
        public int? CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal? CurrencyRate { get; set; }
        public decimal? WbReaPrice { get; set; }
        public decimal? WbReaMoney { get; set; }
        public int? CashierId { get; set; }
        public string CashierName { get; set; }
        public int? CashPcid { get; set; }
        public string CashPcname { get; set; }
        public int? WareShopId { get; set; }
        public string WareShopName { get; set; }
        public int? SalePointId { get; set; }
        public string SalePointName { get; set; }
        public int? SalesmanId { get; set; }
        public string SalesmanName { get; set; }
        public int? MarketerId { get; set; }
        public string MarketerName { get; set; }
        public Guid? MemberId { get; set; }
        public string MemberName { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid? GuiderId { get; set; }
        public string GuiderName { get; set; }
        public long? TicketId { get; set; }
        public long? ZhId { get; set; }
        public string OrderListNo { get; set; }
        public string Memo { get; set; }
        public bool? CanReturnFlag { get; set; }
        public decimal CanReturnNum { get; set; }
        public long? ReturnId { get; set; }
        public bool? StatFlag { get; set; }
        public bool? BdFlag { get; set; }
        public int? SettleStatusId { get; set; }
        public string SettleListNo { get; set; }
        public DateTime? SettleTime { get; set; }
        public int? SettleCashierId { get; set; }
        public string Cdate { get; set; }
        public string Cweek { get; set; }
        public string Cmonth { get; set; }
        public string Cquarter { get; set; }
        public string Cyear { get; set; }
        public string Ctp { get; set; }
        public DateTime? Ctime { get; set; }
        public DateTime? Ltime { get; set; }
        public long? Bid { get; set; }
        public bool? CommitFlag { get; set; }
        public int? ParkId { get; set; }
        public string ParkName { get; set; }
        public bool? ShiftFlag { get; set; }
        public DateTime? ShiftTime { get; set; }
        public Guid? SyncCode { get; set; }
    }
}
