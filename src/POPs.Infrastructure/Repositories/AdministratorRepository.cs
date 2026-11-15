using Microsoft.EntityFrameworkCore;
using POPs.Application.Repositories;
using POPs.Infrastructure.Data;
using POPs.Shared.Models;

namespace POPs.Infrastructure.Repositories;

public class AdministratorRepository(AppDbContext db) : IAdministratorRepository
{
    public Task<Administrator?> GetByLoginAsync(string login) =>
        db.Administrators.FirstOrDefaultAsync(a => a.Login == login);
}
