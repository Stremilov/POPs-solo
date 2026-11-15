using POPs.Application.Repositories;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Application.Services;

public class CourseService
{
    private readonly ICourseRepository _courses;
    private readonly IGroupRepository _groups;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(ICourseRepository courses, IGroupRepository groups, IUnitOfWork unitOfWork)
    {
        _courses = courses;
        _groups = groups;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        var courses = await _courses.GetAllAsync();

        return courses.Select(ToDto).ToList();
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _courses.GetByIdAsync(id);
        return course == null ? null : ToDto(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CourseCreateRequest request)
    {
        var course = new Course
        {
            Number = request.Number,
            Name = request.Name,
            StartYear = request.StartYear,
            EndYear = request.EndYear
        };

        _courses.Add(course);
        await _unitOfWork.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task<CourseDto?> EditCourseAsync(CourseUpdateRequest request)
    {
        var course = await _courses.GetByIdAsync(request.Id);
        if (course == null)
        {
            return null;
        }

        course.Number = request.Number;
        course.Name = request.Name;
        course.StartYear = request.StartYear;
        course.EndYear = request.EndYear;

        await _unitOfWork.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task AddGroupToCourseAsync(int groupId, int courseId)
    {
        var group = await _groups.GetByIdAsync(groupId)
            ?? throw new InvalidOperationException("Группа не найдена.");

        var courseExists = await _courses.ExistsAsync(courseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        group.CourseId = courseId;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string> PromoteStudentsAsync(int courseId)
    {
        var course = await _courses.GetByIdAsync(courseId, includeStudents: true);

        if (course == null)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        var nextNumber = course.Number + 1;
        var nextCourse = await _courses.GetByNumberAsync(nextNumber);

        if (nextCourse == null)
        {
            nextCourse = new Course
            {
                Number = nextNumber,
                Name = $"{nextNumber} курс",
                StartYear = course.StartYear + 1,
                EndYear = course.EndYear + 1
            };
            _courses.Add(nextCourse);
            await _unitOfWork.SaveChangesAsync();
        }

        var movedGroups = 0;
        var movedStudents = 0;

        foreach (var group in course.Groups)
        {
            group.CourseId = nextCourse.Id;
            movedGroups++;
            movedStudents += group.Students.Count(s => s.Status == "Обучается");
        }

        await _unitOfWork.SaveChangesAsync();
        return $"Переведено групп: {movedGroups}, студентов (обучаются): {movedStudents} на курс «{nextCourse.Name}».";
    }

    private static CourseDto ToDto(Course c) => new()
    {
        Id = c.Id,
        Number = c.Number,
        Name = c.Name,
        StartYear = c.StartYear,
        EndYear = c.EndYear,
        GroupsCount = c.Groups?.Count ?? 0
    };
}
