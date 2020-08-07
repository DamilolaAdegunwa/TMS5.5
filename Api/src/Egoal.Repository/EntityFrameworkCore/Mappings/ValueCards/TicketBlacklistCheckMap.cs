using Egoal.ValueCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.ValueCards
{
    public class TicketBlacklistCheckMap : IEntityTypeConfiguration<TicketBlacklistCheck>
    {
        public void Configure(EntityTypeBuilder<TicketBlacklistCheck> entity)
        {
            entity.ToTable("TM_TicketBlacklistCheck");

            entity.HasIndex(e => e.CardNo);

            entity.HasIndex(e => e.TicketCode);

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.CardNo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CheckTypeId)
                .HasColumnName("CheckTypeID");

            entity.Property(e => e.Etime)
                .HasColumnName("ETime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.ParkId)
                .HasColumnName("ParkID");

            entity.Property(e => e.Stime)
                .HasColumnName("STime")
                .HasMaxLength(19)
                .IsUnicode(false);

            entity.Property(e => e.TicketCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TicketId)
                .HasColumnName("TicketID");

            entity.Property(e => e.TradeId)
                .HasColumnName("TradeID");
        }
    }
}
