using Egoal.Wares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Wares
{
    public class WareMap : IEntityTypeConfiguration<Ware>
    {
        public void Configure(EntityTypeBuilder<Ware> entity)
        {
            entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

            entity.ToTable("WM_Ware");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedNever();

            entity.Property(e => e.Barcode).HasMaxLength(50);

            entity.Property(e => e.CanDiscount)
                .IsRequired()
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.Cid).HasColumnName("CID");

            entity.Property(e => e.CostPrice).HasColumnType("decimal(18, 4)");

            entity.Property(e => e.Ctime)
                .HasColumnName("CTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Etime)
                .HasColumnName("ETime")
                .HasMaxLength(10);

            entity.Property(e => e.MaxJsRentFee).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.MemberRetailPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Memo).HasMaxLength(500);

            entity.Property(e => e.MerchantId).HasColumnName("MerchantID");

            entity.Property(e => e.MeterTypeId)
                .HasColumnName("MeterTypeID")
                .HasDefaultValueSql("((1))");

            entity.Property(e => e.MeterTypeName)
                .HasMaxLength(50)
                .HasDefaultValueSql("('按件销售')");

            entity.Property(e => e.Mid).HasColumnName("MID");

            entity.Property(e => e.MinJsRentFee).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Mtime)
                .HasColumnName("MTime")
                .HasColumnType("datetime");

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.ParkId).HasColumnName("ParkID");

            entity.Property(e => e.ParkName).HasMaxLength(50);

            entity.Property(e => e.RentJsPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.RentPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.RentTypeId).HasColumnName("RentTypeID");

            entity.Property(e => e.RentTypeName).HasMaxLength(50);

            entity.Property(e => e.RetailPrice).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.ShopId).HasColumnName("ShopID");

            entity.Property(e => e.SortCode).HasMaxLength(50);

            entity.Property(e => e.Stime)
                .HasColumnName("STime")
                .HasMaxLength(10);

            entity.Property(e => e.StockMoney).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

            entity.Property(e => e.SupplierName).HasMaxLength(50);

            entity.Property(e => e.WareCode).HasMaxLength(20);

            entity.Property(e => e.WareColour).HasMaxLength(50);

            entity.Property(e => e.WareKindId).HasColumnName("WareKindID");

            entity.Property(e => e.WareProducter).HasMaxLength(300);

            entity.Property(e => e.WareRsTypeId).HasColumnName("WareRsTypeID");

            entity.Property(e => e.WareRsTypeName).HasMaxLength(50);

            entity.Property(e => e.WareStandard).HasMaxLength(50);

            entity.Property(e => e.WareTypeId).HasColumnName("WareTypeID");

            entity.Property(e => e.WareTypeName).HasMaxLength(50);

            entity.Property(e => e.WareUnit).HasMaxLength(50);

            entity.Property(e => e.YaJin).HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Zjf).HasMaxLength(50);
        }
    }
}
