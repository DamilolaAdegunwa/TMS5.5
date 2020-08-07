using Egoal.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Tickets
{
    public class TicketGroundSaleMap : IEntityTypeConfiguration<TicketGroundSale>
    {
        public void Configure(EntityTypeBuilder<TicketGroundSale> entity)
        {
            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");

            entity.Property(e => e.ListNo)
                 .HasMaxLength(50);

            entity.Property(e => e.TicketId)
                .HasColumnName("TicketID");

            entity.Property(e => e.GroundId)
                .HasColumnName("GroundID");

            entity.Property(e => e.CashierId)
                .HasColumnName("CashierID");

            entity.Property(e => e.CashPcId)
                .HasColumnName("CashPCID");

            entity.Property(e => e.SalePointId)
                .HasColumnName("SalePointID");

            entity.ToTable("TM_TicketGroundSale");

            entity.HasOne(e => e.TicketSale)
                .WithMany(e => e.TicketGroundSales)
                .HasForeignKey(e => e.TicketId)
                .IsRequired();
        }
    }
}
