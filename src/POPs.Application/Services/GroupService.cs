using POPs.Application.Repositories;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Application.Services;

public class GroupService
{
    private readonly IGroupRepository _groups;
    private readonly ICourseRepository _courses;
    private readonly IStudentRepository _students;
    private readonly IUnitOfWork _unitOfWork;

    public GroupService(IGroupRepository groups, ICourseRepository courses, IStudentRepository students, IUnitOfWork unitOfWork)
    {
        _groups = groups;
        _courses = courses;
        _students = students;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<GroupDto>> GetAllAsync()
    {
        var groups = await _groups.GetAllAsync();

        return groups.Select(ToDto).ToList();
    }

    public async Task<GroupDto?> GetByIdAsync(int id)
    {
        var group = await _groups.GetByIdAsync(id, includeDetails: true);

        return group == null ? null : ToDto(group);
    }

    public async Task<GroupDto> CreateGroupAsync(GroupCreateRequest request)
    {
        var courseExists = await _courses.ExistsAsync(request.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        var group = new Group
        {
            Name = request.Name,
            Curator = request.Curator,
            CourseId = request.CourseId
        };

        _groups.Add(group);
        await _unitOfWork.SaveChangesAsync();

        await _groups.LoadDetailsAsync(group);
        return ToDto(group);
    }

    public async Task<GroupDto?> EditGroupAsync(GroupUpdateRequest request)
    {
        var group = await _groups.GetByIdAsync(request.Id);
        if (group == null)
        {
            return null;
        }

        var courseExists = await _courses.ExistsAsync(request.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        group.Name = request.Name;
        group.Curator = request.Curator;
        group.CourseId = request.CourseId;

        await _unitOfWork.SaveChangesAsync();
        await _groups.LoadDetailsAsync(group);

        return ToDto(group);
    }

    public async Task<bool> DeleteGroupAsync(int id)
    {
        var group = await _groups.GetByIdAsync(id, includeDetails: true);
        if (group == null)
        {
            return false;
        }

        if (group.Students.Count > 0)
        {
            throw new InvalidOperationException("Нельзя удалить группу, в которой есть студенты.");
        }

        _groups.Remove(group);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<GroupDto?> AssignCuratorAsync(int groupId, string curator)
    {
        var group = await _groups.GetByIdAsync(groupId, includeDetails: true);

        if (group == null)
        {
            return null;
        }

        group.Curator = curator;
        await _unitOfWork.SaveChangesAsync();
        return ToDto(group);
    }

    public async Task AddStudentToGroupAsync(int studentId, int groupId)
    {
        var student = await _students.GetByIdAsync(studentId)
            ?? throw new InvalidOperationException("Студент не найден.");

        var groupExists = await _groups.ExistsAsync(groupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        student.GroupId = groupId;
        if (student.Status.StartsWith("Отчислен", StringComparison.OrdinalIgnoreCase))
        {
            student.Status = "Обучается";
        }

        await _unitOfWork.SaveChangesAsync();
    }

    private static GroupDto ToDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        Curator = g.Curator,
        CourseId = g.CourseId,
        CourseName = g.Course?.Name,
        StudentsCount = g.Students?.Count ?? 0
    };
}
