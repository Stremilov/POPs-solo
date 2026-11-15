using POPs.Shared.Models;

namespace POPs.Application.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync(string? search = null);
    Task<List<Student>> GetByGroupAsync(int groupId);
    Task<Student?> GetByIdAsync(int id, bool includeGroup = false);
    Task<Student?> GetByLoginAsync(string login);
    Task<bool> LoginExistsAsync(string login);
    Task<List<Student>> GetContingentAsync();
    Task<int> CountByStatusAsync(string status);
    Task<int> CountByGroupAsync(int groupId);
    Task<int> CountByCourseAsync(int courseId);
    Task<List<Student>> GetForOverviewAsync();
    Task LoadGroupAsync(Student student);
    void Add(Student student);
    void Remove(Student student);
}
