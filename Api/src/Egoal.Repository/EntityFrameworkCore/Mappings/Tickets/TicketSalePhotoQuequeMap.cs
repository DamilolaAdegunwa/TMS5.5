using Egoal.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Tickets
{
    public class TicketSalePhotoQuequeMap : IEntityTypeConfiguration<TicketSalePhotoQueque>
    {
        public void Configure(EntityTypeBuilder<TicketSalePhotoQueque> entity)
        {
            entity.ToTable("TM_TicketSalePhotoQueque");

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.OpTypeId)
                .HasColumnName("OpTypeID");

            entity.Property(e => e.TicketSalePhotoId)
                .HasColumnName("TicketSalePhotoID");

            entity.Property(e => e.TicketTypeId)
                .HasColumnName("TicketTypeID");
        }
    }
}
