using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class WareTypeTypeMap : IEntityTypeConfiguration<WareTypeType>
    {
        public void Configure(EntityTypeBuilder<WareTypeType> entity)
        {
            entity.ToTable("WM_WareTypeType");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SortCode).HasMaxLength(50);
        }
    }
}
