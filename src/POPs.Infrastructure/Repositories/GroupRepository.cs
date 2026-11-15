using Microsoft.EntityFrameworkCore;
using POPs.Application.Repositories;
using POPs.Infrastructure.Data;
using POPs.Shared.Models;

namespace POPs.Infrastructure.Repositories;

public class GroupRepository(AppDbContext db) : IGroupRepository
{
    public Task<List<Group>> GetAllAsync() => db.Groups.Include(g => g.Course)
        .Include(g => g.Students).OrderBy(g => g.Name).ToListAsync();

    public Task<Group?> GetByIdAsync(int id, bool includeDetails = false) =>
        includeDetails
            ? db.Groups.Include(g => g.Course).Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == id)
            : db.Groups.FindAsync(id).AsTask();

    public Task<bool> ExistsAsync(int id) => db.Groups.AnyAsync(g => g.Id == id);
    public Task<int> CountAsync() => db.Groups.CountAsync();

    public async Task LoadDetailsAsync(Group group)
    {
        await db.Entry(group).Reference(g => g.Course).LoadAsync();
        await db.Entry(group).Collection(g => g.Students).LoadAsync();
    }

    public void Add(Group group) => db.Groups.Add(group);
    public void Remove(Group group) => db.Groups.Remove(group);
}
