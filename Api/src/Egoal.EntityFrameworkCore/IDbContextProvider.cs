using Egoal.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace Egoal
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext GetDbContext();
        UnitOfWorkOptions GetOptions();
    }
}
