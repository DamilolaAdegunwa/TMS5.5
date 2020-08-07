using Egoal.ValueCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.ValueCards
{
    public class MemberAccountMap : IEntityTypeConfiguration<MemberAccount>
    {
        public void Configure(EntityTypeBuilder<MemberAccount> entity)
        {
            entity.ToTable("MM_MemberAccount");

            entity.HasIndex(e => e.ListNo)
                .HasName("IX_MemberAccount_ListNo");

            entity.HasIndex(e => e.TradeId)
                .HasName("IX_MemberAccount_TradeID");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedNever();

            entity.Property(e => e.AccountTypeId)
                .HasColumnName("AccountTypeID")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.ActiveFlagName)
                .HasMaxLength(50);

            entity.Property(e => e.Bid)
                .HasColumnName("BID");

            entity.Property(e => e.CashPcid)
                .HasColumnName("CashPCID");

            entity.Property(e => e.CashPcname)
                .HasColumnName("CashPCName")
                .HasMaxLength(50);

            entity.Property(e => e.CashierId)
                .HasColumnName("CashierID");

            entity.Property(e => e.CashierName)
                .HasMaxLength(50);

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");

            entity.Property(e => e.GuiderId)
                .HasColumnName("GuiderID");

            entity.Property(e => e.ListNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Ltime)
                .HasColumnName("LTime")
                .HasColumnType("datetime");

            entity.Property(e => e.MemberId)
                .HasColumnName("MemberID");

            entity.Property(e => e.Memo)
                .HasMaxLength(100);

            entity.Property(e => e.Name)
                .HasMaxLength(100);

            entity.Property(e => e.ParkId)
                .HasColumnName("ParkID");

            entity.Property(e => e.ParkName)
                .HasMaxLength(50);

            entity.Property(e => e.Pwd)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.Property(e => e.SalePointId)
                .HasColumnName("SalePointID");

            entity.Property(e => e.SalePointName)
                .HasMaxLength(50);

            entity.Property(e => e.Salt)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TicketStatusId)
                .HasColumnName("TicketStatusID");

            entity.Property(e => e.TicketStatusName)
                .HasMaxLength(50);

            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");
        }
    }
}
