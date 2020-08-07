using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class ShopMap : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> entity)
        {
            entity.ToTable("WM_Shop");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.MerchantId).HasColumnName("MerchantID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SalePointId).HasColumnName("SalePointID");

            entity.Property(e => e.ShopCode).HasMaxLength(50);

            entity.Property(e => e.ShopTypeId).HasColumnName("ShopTypeID");

            entity.Property(e => e.SortCode).HasMaxLength(50);

            entity.Property(e => e.WareHouseId).HasColumnName("WareHouseID");
        }
    }
}
