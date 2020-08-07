using System;

namespace Egoal.Auditing
{
    public class AuditInfo
    {
        public long? UserId { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public string ReturnValue { get; set; }
        public DateTime ExecutionTime { get; set; }
        public int ExecutionDuration { get; set; }
        public string ClientIpAddress { get; set; }
        public string ClientName { get; set; }
        public string BrowserInfo { get; set; }
        public string CustomData { get; set; }
        public Exception Exception { get; set; }
    }
}
