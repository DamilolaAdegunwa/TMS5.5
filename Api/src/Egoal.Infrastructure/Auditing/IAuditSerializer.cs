namespace Egoal.Auditing
{
    public interface IAuditSerializer
    {
        string Serialize(object obj);
    }
}
