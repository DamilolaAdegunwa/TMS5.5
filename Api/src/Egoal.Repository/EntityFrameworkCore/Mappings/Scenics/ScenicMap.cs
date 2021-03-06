using Egoal.Scenics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Scenics
{
    public class ScenicMap : IEntityTypeConfiguration<Scenic>
    {
        public void Configure(EntityTypeBuilder<Scenic> entity)
        {
            entity.ToTable("VM_Scenic");

            entity.Property(e => e.ScenicName)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.OpenTime)
                .HasMaxLength(5)
                .IsRequired()
                .IsUnicode(false);

            entity.Property(e => e.CloseTime)
                .HasMaxLength(5)
                .IsRequired()
                .IsUnicode(false);

            entity.Property(e => e.ScenicIntro)
                .IsRequired();

            entity.Property(e => e.NoticeTitle)
                .HasMaxLength(50);

            entity.Property(e => e.Address)
                .HasMaxLength(200);
        }
    }
}
