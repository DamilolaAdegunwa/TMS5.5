using Egoal.TicketTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.TicketTypes
{
    public class TicketTypeGroundPriceMap : IEntityTypeConfiguration<TicketTypeGroundPrice>
    {
        public void Configure(EntityTypeBuilder<TicketTypeGroundPrice> entity)
        {
            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");

            entity.Property(e => e.GroundId)
                .HasColumnName("GroundID");

            entity.ToTable("TM_TicketTypeGroundPrice");
        }
    }
}
