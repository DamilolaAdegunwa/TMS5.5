using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.Auditing
{
    public class AuditLogMap : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> entity)
        {
            entity.Property(e => e.ServiceName)
                .HasMaxLength(AuditLog.MaxServiceNameLength);

            entity.Property(e => e.MethodName)
                .HasMaxLength(AuditLog.MaxMethodNameLength);

            entity.Property(e => e.Parameters)
                .HasMaxLength(AuditLog.MaxParametersLength);

            entity.Property(e => e.ReturnValue)
                .HasMaxLength(AuditLog.MaxReturnValueLength);

            entity.Property(e => e.ExecutionTime)
                .HasColumnType("datetime");

            entity.Property(e => e.ClientIpAddress)
                .HasMaxLength(AuditLog.MaxClientIpAddressLength);

            entity.Property(e => e.ClientName)
                .HasMaxLength(AuditLog.MaxClientNameLength);

            entity.Property(e => e.BrowserInfo)
                .HasMaxLength(AuditLog.MaxBrowserInfoLength);

            entity.Property(e => e.Exception)
                .HasMaxLength(AuditLog.MaxExceptionLength);

            entity.Property(e => e.CustomData)
                .HasMaxLength(AuditLog.MaxCustomDataLength);

            entity.ToTable("AuditLog");
        }
    }
}
