using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Wares
{
    public class WareIODetailMap : IEntityTypeConfiguration<WareIODetail>
    {
        public void Configure(EntityTypeBuilder<WareIODetail> entity)
        {
            entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

            entity.ToTable("WM_WareIODetail");

            entity.HasIndex(e => e.Ctime);

            entity.HasIndex(e => e.ListNo);

            entity.HasIndex(e => e.MemberAccountId);

            entity.HasIndex(e => e.SettleListNo);

            entity.HasIndex(e => e.SettleTime);

            entity.HasIndex(e => e.SyncCode);

            entity.HasIndex(e => e.TradeId)
                .ForSqlServerIsClustered();

            entity.HasIndex(e => e.WareId);

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Barcode).HasMaxLength(50);

            entity.Property(e => e.Bid).HasColumnName("BID");

            entity.Property(e => e.CanReturnNum).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.CashPcid).HasColumnName("CashPCID");

            entity.Property(e => e.CashPcname)
                .HasColumnName("CashPCName")
                .HasMaxLength(50);

            entity.Property(e => e.CashierId).HasColumnName("CashierID");

            entity.Property(e => e.CashierName).HasMaxLength(50);

            entity.Property(e => e.Cdate)
                .HasColumnName("CDate")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Cmonth)
                .HasColumnName("CMonth")
                .HasMaxLength(7)
                .IsUnicode(false);

            entity.Property(e => e.CostMoney).HasColumnType("decimal(18, 4)");

            entity.Property(e => e.CostPrice).HasColumnType("decimal(18, 4)");

            entity.Property(e => e.Cquarter)
                .HasColumnName("CQuarter")
                .HasMaxLength(6)
                .IsUnicode(false);

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Ctp)
                .HasColumnName("CTP")
                .HasMaxLength(5)
                .IsUnicode(false);

            entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");

            entity.Property(e => e.CurrencyName).HasMaxLength(50);

            entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.Property(e => e.CustomerName).HasMaxLength(50);

            entity.Property(e => e.Cweek)
                .HasColumnName("CWeek")
                .HasMaxLength(3);

            entity.Property(e => e.Cyear)
                .HasColumnName("CYear")
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.Property(e => e.CzkCardNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CzkId).HasColumnName("CzkID");

            entity.Property(e => e.CzkOwner).HasMaxLength(50);

            entity.Property(e => e.CzkOwnerTel)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CzkTicketCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.DiscountApproverId).HasColumnName("DiscountApproverID");

            entity.Property(e => e.DiscountApproverName).HasMaxLength(50);

            entity.Property(e => e.DiscountRate).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.DiscountTypeId).HasColumnName("DiscountTypeID");

            entity.Property(e => e.DiscountTypeName).HasMaxLength(50);

            entity.Property(e => e.GuiderId).HasColumnName("GuiderID");

            entity.Property(e => e.GuiderName).HasMaxLength(50);

            entity.Property(e => e.GuiderSharingMoney)
                .HasColumnType("decimal(18, 2)")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.Ionum)
                .HasColumnName("IONum")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.IonumAbs)
                .HasColumnName("IONumAbs")
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ListNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Ltime)
                .HasColumnName("LTime")
                .HasColumnType("datetime");

            entity.Property(e => e.MarketerId).HasColumnName("MarketerID");

            entity.Property(e => e.MarketerName).HasMaxLength(50);

            entity.Property(e => e.MemberAccountId).HasColumnName("MemberAccountID");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.Property(e => e.MemberName).HasMaxLength(50);

            entity.Property(e => e.Memo).HasMaxLength(100);

            entity.Property(e => e.OrderListNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.ParkId).HasColumnName("ParkID");

            entity.Property(e => e.ParkName).HasMaxLength(50);

            entity.Property(e => e.PayTypeId).HasColumnName("PayTypeID");

            entity.Property(e => e.PayTypeName).HasMaxLength(50);

            entity.Property(e => e.Profit).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ReaMoney).HasColumnType("decimal(18, 4)");

            entity.Property(e => e.ReaPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.RentMinutes).HasDefaultValueSql("((0))");

            entity.Property(e => e.RentPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ReturnId).HasColumnName("ReturnID");

            entity.Property(e => e.SalePointId).HasColumnName("SalePointID");

            entity.Property(e => e.SalePointName).HasMaxLength(50);

            entity.Property(e => e.SalesmanId).HasColumnName("SalesmanID");

            entity.Property(e => e.SalesmanName).HasMaxLength(50);

            entity.Property(e => e.SettleCashierId).HasColumnName("SettleCashierID");

            entity.Property(e => e.SettleListNo).HasMaxLength(50);

            entity.Property(e => e.SettleStatusId)
                .HasColumnName("SettleStatusID")
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.SettleTime).HasColumnType("datetime");

            entity.Property(e => e.ShiftFlag).HasDefaultValueSql("((0))");

            entity.Property(e => e.ShiftTime).HasColumnType("datetime");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.Property(e => e.SupplierName).HasMaxLength(50);

            entity.Property(e => e.TicketId).HasColumnName("TicketID");

            entity.Property(e => e.TradeId).HasColumnName("TradeID");

            entity.Property(e => e.TradeTypeId).HasColumnName("TradeTypeID");

            entity.Property(e => e.TradeTypeName).HasMaxLength(50);

            entity.Property(e => e.WareHouseId).HasColumnName("WareHouseID");

            entity.Property(e => e.WareId).HasColumnName("WareID");

            entity.Property(e => e.WareName).HasMaxLength(50);

            entity.Property(e => e.WareShopId).HasColumnName("WareShopID");

            entity.Property(e => e.WareShopName).HasMaxLength(50);

            entity.Property(e => e.WbReaMoney).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.WbReaPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.YaJin).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ZhId).HasColumnName("ZhID");
        }
    }
}
