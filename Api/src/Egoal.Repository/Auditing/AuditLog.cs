using Egoal.Domain.Entities;
using Egoal.Extensions;
using System;

namespace Egoal.Auditing
{
    public class AuditLog : Entity<long>
    {
        public const int MaxServiceNameLength = 256;
        public const int MaxMethodNameLength = 256;
        public const int MaxParametersLength = 1024;
        public const int MaxReturnValueLength = 1024;
        public const int MaxClientIpAddressLength = 64;
        public const int MaxClientNameLength = 128;
        public const int MaxBrowserInfoLength = 512;
        public const int MaxExceptionLength = 2000;
        public const int MaxCustomDataLength = 2000;

        public virtual long? UserId { get; set; }
        public virtual string ServiceName { get; set; }
        public virtual string MethodName { get; set; }
        public virtual string Parameters { get; set; }
        public virtual string ReturnValue { get; set; }
        public virtual DateTime ExecutionTime { get; set; }
        public virtual int ExecutionDuration { get; set; }
        public virtual string ClientIpAddress { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string BrowserInfo { get; set; }
        public virtual string Exception { get; set; }
        public virtual string CustomData { get; set; }

        public static AuditLog FromAuditInfo(AuditInfo auditInfo)
        {
            return new AuditLog
            {
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
                Parameters = auditInfo.Parameters.TruncateWithPostfix(MaxParametersLength),
                ReturnValue = auditInfo.ReturnValue.TruncateWithPostfix(MaxReturnValueLength),
                ExecutionTime = auditInfo.ExecutionTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                ClientIpAddress = auditInfo.ClientIpAddress.TruncateWithPostfix(MaxClientIpAddressLength),
                ClientName = auditInfo.ClientName.TruncateWithPostfix(MaxClientNameLength),
                BrowserInfo = auditInfo.BrowserInfo.TruncateWithPostfix(MaxBrowserInfoLength),
                Exception = auditInfo.Exception?.Message?.TruncateWithPostfix(MaxExceptionLength),
                CustomData = auditInfo.CustomData.TruncateWithPostfix(MaxCustomDataLength)
            };
        }
    }
}
