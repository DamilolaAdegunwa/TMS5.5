using Egoal.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Tickets
{
    public class TicketSalePhotoMap : IEntityTypeConfiguration<TicketSalePhoto>
    {
        public void Configure(EntityTypeBuilder<TicketSalePhoto> entity)
        {
            entity.ToTable("TM_TicketSalePhoto");

            entity.HasIndex(e => e.Etime)
                .HasName("IX_TicketSalePhoto_ETime");

            entity.HasIndex(e => e.TicketId)
                .HasName("IX_TicketSalePhoto_TicketID");

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Etime)
                .HasColumnName("ETime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.Name)
                .HasMaxLength(50);

            entity.Property(e => e.ParkId)
                .HasColumnName("ParkID");

            entity.Property(e => e.Stime)
                .HasColumnName("STime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.TicketId)
                .HasColumnName("TicketID");

            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");

            entity.Property(e => e.TicketTypeTypeId)
                .HasColumnName("TicketTypeTypeID");

            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");

            entity.Property(e => e.CashierId)
                .HasColumnName("CashierID");

            entity.Property(e => e.SalePointId)
                .HasColumnName("SalePointID");

            entity.Property(e => e.RegSourceId)
                .HasColumnName("RegSourceID");

            entity.Property(e => e.RegCashPcId)
                .HasColumnName("RegCashPCID");

            entity.Property(e => e.RegGateId)
                .HasColumnName("RegGateID");

            entity.HasOne(e => e.TicketSale)
                .WithMany(e => e.TicketSalePhotos)
                .HasForeignKey(e => e.TicketId);
        }
    }
}
