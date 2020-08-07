using Egoal.TicketTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.TicketTypes
{
    public class TicketTypeGroundChangCiMap : IEntityTypeConfiguration<TicketTypeGroundChangCi>
    {
        public void Configure(EntityTypeBuilder<TicketTypeGroundChangCi> entity)
        {
            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");

            entity.Property(e => e.GroundId)
                .HasColumnName("GroundID");

            entity.Property(e => e.ChangCiId)
                .HasColumnName("ChangCiID");

            entity.ToTable("TM_TicketTypeGroundChangCi");
        }
    }
}
