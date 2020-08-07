using Egoal.Dependency;
using Egoal.Domain.Repositories;
using System.Threading.Tasks;

namespace Egoal.Auditing
{
    public class AuditingStore : IAuditingStore, ITransientDependency
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;

        public AuditingStore(IRepository<AuditLog,long> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task SaveAsync(AuditInfo auditInfo)
        {
            var auditLog = AuditLog.FromAuditInfo(auditInfo);
            await _auditLogRepository.InsertAsync(auditLog);
        }
    }
}
