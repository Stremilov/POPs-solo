using POPs.Shared.Models;

namespace POPs.Application.Repositories;

public interface IGroupRepository
{
    Task<List<Group>> GetAllAsync();
    Task<Group?> GetByIdAsync(int id, bool includeDetails = false);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task LoadDetailsAsync(Group group);
    void Add(Group group);
    void Remove(Group group);
}
