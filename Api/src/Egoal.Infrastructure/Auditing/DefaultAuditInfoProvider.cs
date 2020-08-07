using Egoal.Extensions;

namespace Egoal.Auditing
{
    public class DefaultAuditInfoProvider : IAuditInfoProvider
    {
        private readonly IClientInfoProvider _clientInfoProvider;

        public DefaultAuditInfoProvider(IClientInfoProvider clientInfoProvider)
        {
            _clientInfoProvider = clientInfoProvider;
        }

        public virtual void Fill(AuditInfo auditInfo)
        {
            if (auditInfo.ClientIpAddress.IsNullOrEmpty())
            {
                auditInfo.ClientIpAddress = _clientInfoProvider.ClientIpAddress;
            }

            if (auditInfo.BrowserInfo.IsNullOrEmpty())
            {
                auditInfo.BrowserInfo = _clientInfoProvider.BrowserInfo;
            }

            if (auditInfo.ClientName.IsNullOrEmpty())
            {
                auditInfo.ClientName = _clientInfoProvider.ComputerName;
            }
        }
    }
}
