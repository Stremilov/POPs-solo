using POPs.Application.Repositories;
using POPs.Infrastructure.Data;

namespace POPs.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => db.SaveChangesAsync();
}
