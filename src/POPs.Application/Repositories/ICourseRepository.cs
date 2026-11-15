using POPs.Shared.Models;

namespace POPs.Application.Repositories;

public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id, bool includeStudents = false);
    Task<Course?> GetByNumberAsync(int number);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    void Add(Course course);
}
