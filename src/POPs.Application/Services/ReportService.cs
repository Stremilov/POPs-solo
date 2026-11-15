using POPs.Application.Repositories;
using POPs.Shared.Dtos;

namespace POPs.Application.Services;

public class ReportService
{
    private readonly IStudentRepository _students;
    private readonly IGroupRepository _groups;
    private readonly ICourseRepository _courses;

    public ReportService(IStudentRepository students, IGroupRepository groups, ICourseRepository courses)
    {
        _students = students;
        _groups = groups;
        _courses = courses;
    }

    public async Task<List<ContingentItemDto>> GetContingentAsync()
    {
        var students = await _students.GetContingentAsync();

        return students.Select(s => new ContingentItemDto
        {
            StudentId = s.Id,
            FullName = s.FullName,
            Status = s.Status,
            GroupName = s.Group?.Name ?? "—",
            CourseName = s.Group?.Course?.Name ?? "—"
        }).ToList();
    }

    public async Task<int> CountByStatusAsync(string status)
    {
        return await _students.CountByStatusAsync(status);
    }

    public async Task<int> CountByGroupAsync(int groupId)
    {
        return await _students.CountByGroupAsync(groupId);
    }

    public async Task<int> CountByCourseAsync(int courseId)
    {
        return await _students.CountByCourseAsync(courseId);
    }

    public async Task<GroupReportDto?> GetGroupStatisticsAsync(int groupId)
    {
        var group = await _groups.GetByIdAsync(groupId, includeDetails: true);

        if (group == null)
        {
            return null;
        }

        var students = group.Students.OrderBy(s => s.FullName).ToList();

        if (students.Count == 0)
        {
            return new GroupReportDto
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Curator = group.Curator,
                CourseName = group.Course?.Name ?? "—",
                IsEmpty = true,
                TotalStudents = 0,
                ByStatus = new Dictionary<string, int>(),
                Students = new List<StudentDto>()
            };
        }

        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new GroupReportDto
        {
            GroupId = group.Id,
            GroupName = group.Name,
            Curator = group.Curator,
            CourseName = group.Course?.Name ?? "—",
            IsEmpty = false,
            TotalStudents = students.Count,
            ByStatus = byStatus,
            Students = students.Select(s => new StudentDto
            {
                Id = s.Id,
                GroupId = s.GroupId,
                GroupName = group.Name,
                FullName = s.FullName,
                BirthDate = s.BirthDate,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                Status = s.Status,
                PreviousEducation = s.PreviousEducation,
                GraduationYear = s.GraduationYear,
                Login = s.Login
            }).ToList()
        };
    }

    public async Task<CourseStatisticsDto?> GetCourseStatisticsAsync(int courseId)
    {
        var course = await _courses.GetByIdAsync(courseId, includeStudents: true);

        if (course == null)
        {
            return null;
        }

        var students = course.Groups.SelectMany(g => g.Students).ToList();
        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new CourseStatisticsDto
        {
            CourseId = course.Id,
            CourseName = course.Name,
            GroupsCount = course.Groups.Count,
            StudentsCount = students.Count,
            ByStatus = byStatus
        };
    }

    public async Task<OverviewStatisticsDto> GetOverviewAsync()
    {
        var students = await _students.GetForOverviewAsync();
        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new OverviewStatisticsDto
        {
            TotalStudents = students.Count,
            TotalGroups = await _groups.CountAsync(),
            TotalCourses = await _courses.CountAsync(),
            ByStatus = byStatus
        };
    }
}
