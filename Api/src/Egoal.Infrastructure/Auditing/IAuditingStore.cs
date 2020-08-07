using System.Threading.Tasks;

namespace Egoal.Auditing
{
    public interface IAuditingStore
    {
        Task SaveAsync(AuditInfo auditInfo);
    }
}