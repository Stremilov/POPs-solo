using POPs.Shared.Models;

namespace POPs.Application.Repositories;

public interface IAdministratorRepository
{
    Task<Administrator?> GetByLoginAsync(string login);
}
