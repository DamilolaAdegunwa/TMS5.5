using Egoal.Extensions;

namespace Egoal.Auditing
{
    public class JsonAuditSerializer : IAuditSerializer
    {
        public string Serialize(object obj)
        {
            return obj.ToJson();
        }
    }
}
