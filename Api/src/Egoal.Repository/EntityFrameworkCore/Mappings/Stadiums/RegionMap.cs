using Egoal.Stadiums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Stadiums
{
    public class RegionMap : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> entity)
        {
            entity.ToTable("SS_Region");

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.FloorId)
                .HasColumnName("FloorID");

            entity.Property(e => e.InDoor)
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.OutDoor)
                .HasMaxLength(50);

            entity.Property(e => e.SeatCodePrefix)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.SeatTypeId)
                .HasColumnName("SeatTypeID");

            entity.Property(e => e.SortCode)
                .HasMaxLength(50);

            entity.Property(e => e.StadiumId)
                .HasColumnName("StadiumID");

            entity.Property(e => e.Xcount)
                .HasColumnName("XCount");

            entity.Property(e => e.Ycount)
                .HasColumnName("YCount");
        }
    }
}
