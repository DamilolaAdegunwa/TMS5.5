using Egoal.ValueCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.ValueCards
{
    public class CzkCztcMap : IEntityTypeConfiguration<CzkCztc>
    {
        public void Configure(EntityTypeBuilder<CzkCztc> entity)
        {
            entity.ToTable("MM_CzkCztc");

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.SortCode)
                .HasMaxLength(50);
        }
    }
}
