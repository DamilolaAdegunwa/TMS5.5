using Egoal.ValueCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.ValueCards
{
    public class CzkDetailMap : IEntityTypeConfiguration<CzkDetail>
    {
        public void Configure(EntityTypeBuilder<CzkDetail> entity)
        {
            entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

            entity.ToTable("MM_CzkDetail");

            entity.HasIndex(e => e.Ctime)
                .ForSqlServerIsClustered();

            entity.HasIndex(e => e.ListNo);

            entity.HasIndex(e => e.MemberAccountId);

            entity.HasIndex(e => e.SaleSyncCode);

            entity.HasIndex(e => e.TicketCheckSyncCode);

            entity.HasIndex(e => e.TicketCode);

            entity.HasIndex(e => e.TradeId);

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.AccountTypeId)
                .HasColumnName("AccountTypeID")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Bid)
                .HasColumnName("BID");

            entity.Property(e => e.CardNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CashPcid)
                .HasColumnName("CashPCID");

            entity.Property(e => e.CashPcname)
                .HasColumnName("CashPCName")
                .HasMaxLength(50);

            entity.Property(e => e.CashierId)
                .HasColumnName("CashierID");

            entity.Property(e => e.CashierName)
                .HasMaxLength(50);

            entity.Property(e => e.Cdate)
                .HasColumnName("CDate")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.Cmonth)
                .HasColumnName("CMonth")
                .HasMaxLength(7)
                .IsUnicode(false);

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

            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");

            entity.Property(e => e.Cweek)
                .HasColumnName("CWeek")
                .HasMaxLength(3);

            entity.Property(e => e.Cyear)
                .HasColumnName("CYear")
                .HasMaxLength(4)
                .IsUnicode(false);

            entity.Property(e => e.CzkConsumeTypeId)
                .HasColumnName("CzkConsumeTypeID");

            entity.Property(e => e.CzkConsumeTypeName)
                .HasMaxLength(50);

            entity.Property(e => e.CzkCztcId)
                .HasColumnName("CzkCztcID");

            entity.Property(e => e.CzkCztcName)
                .HasMaxLength(50);

            entity.Property(e => e.CzkId)
                .HasColumnName("CzkID");

            entity.Property(e => e.CzkOpTypeId)
                .HasColumnName("CzkOpTypeID");

            entity.Property(e => e.CzkOpTypeName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CzkRechargeTypeId)
                .HasColumnName("CzkRechargeTypeID");

            entity.Property(e => e.CzkRechargeTypeName)
                .HasMaxLength(50);

            entity.Property(e => e.GuiderId)
                .HasColumnName("GuiderID");

            entity.Property(e => e.GuiderSharingMoney)
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.ListNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Ltime)
                .HasColumnName("LTime")
                .HasColumnType("datetime");

            entity.Property(e => e.MemberAccountId)
                .HasColumnName("MemberAccountID");

            entity.Property(e => e.MemberId)
                .HasColumnName("MemberID");

            entity.Property(e => e.MemberName)
                .HasMaxLength(50);

            entity.Property(e => e.MemberTel)
                .HasMaxLength(50);

            entity.Property(e => e.Memo)
                .HasMaxLength(100);

            entity.Property(e => e.NewEtime)
                .HasColumnName("NewETime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.OldEtime)
                .HasColumnName("OldETime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.ParkId)
                .HasColumnName("ParkID");

            entity.Property(e => e.ParkName)
                .HasMaxLength(50);

            entity.Property(e => e.ReturnCardMoney)
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.ReturnCzkDetailId)
                .HasColumnName("ReturnCzkDetailID");

            entity.Property(e => e.ReturnFreeMoney)
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.ReturnGameMoney)
                .HasDefaultValueSql("((0))");

            entity.Property(e => e.SalePointId)
                .HasColumnName("SalePointID");

            entity.Property(e => e.SalePointName)
                .HasMaxLength(50);

            entity.Property(e => e.SalesmanId)
                .HasColumnName("SalesmanID");

            entity.Property(e => e.SalesmanName)
                .HasMaxLength(50);

            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");

            entity.Property(e => e.TicketTypeName)
                .HasMaxLength(50);

            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");
        }
    }
}
