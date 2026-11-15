using Microsoft.EntityFrameworkCore;
using POPs.Application.Repositories;
using POPs.Infrastructure.Data;
using POPs.Shared.Models;

namespace POPs.Infrastructure.Repositories;

public class CourseRepository(AppDbContext db) : ICourseRepository
{
    public Task<List<Course>> GetAllAsync() => db.Courses.Include(c => c.Groups)
        .OrderBy(c => c.Number).ToListAsync();

    public Task<Course?> GetByIdAsync(int id, bool includeStudents = false) =>
        includeStudents
            ? db.Courses.Include(c => c.Groups).ThenInclude(g => g.Students)
                .FirstOrDefaultAsync(c => c.Id == id)
            : db.Courses.Include(c => c.Groups).FirstOrDefaultAsync(c => c.Id == id);

    public Task<Course?> GetByNumberAsync(int number) =>
        db.Courses.FirstOrDefaultAsync(c => c.Number == number);

    public Task<bool> ExistsAsync(int id) => db.Courses.AnyAsync(c => c.Id == id);
    public Task<int> CountAsync() => db.Courses.CountAsync();
    public void Add(Course course) => db.Courses.Add(course);
}
