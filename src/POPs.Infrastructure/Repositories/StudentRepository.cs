using Microsoft.EntityFrameworkCore;
using POPs.Application.Repositories;
using POPs.Infrastructure.Data;
using POPs.Shared.Models;

namespace POPs.Infrastructure.Repositories;

public class StudentRepository(AppDbContext db) : IStudentRepository
{
    public async Task<List<Student>> GetAllAsync(string? search = null)
    {
        var query = db.Students.Include(s => s.Group).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s => s.FullName.ToLower().Contains(term)
                || s.Email.ToLower().Contains(term) || s.Status.ToLower().Contains(term));
        }
        return await query.OrderBy(s => s.FullName).ToListAsync();
    }

    public Task<List<Student>> GetByGroupAsync(int groupId) =>
        db.Students.Include(s => s.Group).Where(s => s.GroupId == groupId)
            .OrderBy(s => s.FullName).ToListAsync();

    public Task<Student?> GetByIdAsync(int id, bool includeGroup = false) =>
        includeGroup
            ? db.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id)
            : db.Students.FindAsync(id).AsTask();

    public Task<Student?> GetByLoginAsync(string login) =>
        db.Students.FirstOrDefaultAsync(s => s.Login == login);

    public Task<bool> LoginExistsAsync(string login) =>
        db.Students.AnyAsync(s => s.Login == login);

    public Task<List<Student>> GetContingentAsync() =>
        db.Students.Include(s => s.Group)!.ThenInclude(g => g!.Course)
            .OrderBy(s => s.FullName).ToListAsync();

    public Task<int> CountByStatusAsync(string status) =>
        db.Students.CountAsync(s => s.Status == status);

    public Task<int> CountByGroupAsync(int groupId) =>
        db.Students.CountAsync(s => s.GroupId == groupId);

    public Task<int> CountByCourseAsync(int courseId) =>
        db.Students.CountAsync(s => s.Group != null && s.Group.CourseId == courseId);

    public Task<List<Student>> GetForOverviewAsync() => db.Students.ToListAsync();

    public Task LoadGroupAsync(Student student) =>
        db.Entry(student).Reference(s => s.Group).LoadAsync();

    public void Add(Student student) => db.Students.Add(student);
    public void Remove(Student student) => db.Students.Remove(student);
}
