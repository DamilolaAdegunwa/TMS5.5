using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class ShopTypeMap : IEntityTypeConfiguration<ShopType>
    {
        public void Configure(EntityTypeBuilder<ShopType> entity)
        {
            entity.ToTable("WM_ShopType");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SortCode).HasMaxLength(50);
        }
    }
}
