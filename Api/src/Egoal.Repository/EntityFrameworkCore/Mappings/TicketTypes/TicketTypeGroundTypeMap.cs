using Egoal.TicketTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.TicketTypes
{
    public class TicketTypeGroundTypeMap : IEntityTypeConfiguration<TicketTypeGroundType>
    {
        public void Configure(EntityTypeBuilder<TicketTypeGroundType> entity)
        {
            entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

            entity.ToTable("TM_TicketTypeGroundType");

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.GroundTypeId)
                .HasColumnName("GroundTypeID");

            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");
        }
    }
}
