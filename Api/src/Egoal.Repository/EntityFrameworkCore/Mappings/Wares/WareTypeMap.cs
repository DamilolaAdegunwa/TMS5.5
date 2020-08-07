using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class WareTypeMap : IEntityTypeConfiguration<WareType>
    {
        public void Configure(EntityTypeBuilder<WareType> entity)
        {
            entity.ToTable("WM_WareType");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SortCode).HasMaxLength(50);

            entity.Property(e => e.WareTypeTypeId).HasColumnName("WareTypeTypeID");
        }
    }
}
