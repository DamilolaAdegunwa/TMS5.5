using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Trades
{
    public class SupplierMap : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> entity)
        {
            entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

            entity.ToTable("WM_Supplier");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedNever();

            entity.Property(e => e.Address).HasMaxLength(100);

            entity.Property(e => e.Cid).HasColumnName("CID");

            entity.Property(e => e.Code).HasMaxLength(50);

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Linkman).HasMaxLength(50);

            entity.Property(e => e.Memo).HasMaxLength(100);

            entity.Property(e => e.Mid).HasColumnName("MID");

            entity.Property(e => e.Mtime)
                .HasColumnName("MTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.ParkId).HasColumnName("ParkID");

            entity.Property(e => e.ParkName).HasMaxLength(50);

            entity.Property(e => e.SortCode).HasMaxLength(50);

            entity.Property(e => e.SupplierStatusId).HasColumnName("SupplierStatusID");

            entity.Property(e => e.SupplierStatusName).HasMaxLength(50);

            entity.Property(e => e.SupplierTypeId).HasColumnName("SupplierTypeID");

            entity.Property(e => e.SupplierTypeName).HasMaxLength(50);

            entity.Property(e => e.Tel).HasMaxLength(50);

            entity.Property(e => e.Zjf).HasMaxLength(50);
        }
    }
}
