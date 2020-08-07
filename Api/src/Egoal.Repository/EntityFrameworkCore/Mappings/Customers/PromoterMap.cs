using Egoal.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Customers
{
    public class PromoterMap : IEntityTypeConfiguration<Promoter>
    {
        public void Configure(EntityTypeBuilder<Promoter> entity)
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.UserName)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(50);

            entity.Property(e => e.Password)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(128);

            entity.Property(e => e.Salt)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(50);

            entity.Property(e => e.Mobile)
                .IsUnicode(false)
                .HasMaxLength(20);

            entity.Property(e => e.CertNo)
                .HasMaxLength(50);

            entity.ToTable("CM_Promoter");

            entity.HasIndex(e => e.UserName)
                .IsUnique()
                .HasName("IX_CM_Promoter_UserName");
        }
    }
}
