using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class MerchantMap : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> entity)
        {
            entity.ToTable("WM_Merchant");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.MerchantCode)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.MerchantName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SortCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
