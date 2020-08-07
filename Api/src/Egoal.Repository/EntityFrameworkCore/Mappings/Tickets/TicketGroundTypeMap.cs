using Egoal.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Tickets
{
    public class TicketGroundTypeMap : IEntityTypeConfiguration<TicketGroundType>
    {
        public void Configure(EntityTypeBuilder<TicketGroundType> entity)
        {
            entity.ToTable("TM_TicketGroundType");

            entity.HasIndex(e => e.CardNo);

            entity.HasIndex(e => e.CertNo);

            entity.HasIndex(e => e.TicketId);

            entity.HasIndex(e => e.TradeId);

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.CardNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CertNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Etime)
                .HasColumnName("ETime")
                .HasColumnType("datetime");

            entity.Property(e => e.GroundTypeId)
                .HasColumnName("GroundTypeID");

            entity.Property(e => e.ParkId)
                .HasColumnName("ParkID");

            entity.Property(e => e.Stime)
                .HasColumnName("STime")
                .HasColumnType("datetime");

            entity.Property(e => e.TicketId)
                .HasColumnName("TicketID");

            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");

            entity.HasOne(e => e.TicketSale)
               .WithMany(e => e.TicketGroundTypes)
               .HasForeignKey(e => e.TicketId)
               .IsRequired();
        }
    }
}
