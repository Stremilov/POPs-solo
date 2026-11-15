using POPs.Application.Repositories;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Application.Services;

public class StudentService
{
    private readonly IStudentRepository _students;
    private readonly IGroupRepository _groups;
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IStudentRepository students, IGroupRepository groups, IUnitOfWork unitOfWork)
    {
        _students = students;
        _groups = groups;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StudentDto>> GetAllAsync(string? search = null)
    {
        var list = await _students.GetAllAsync(search);
        return list.Select(ToDto).ToList();
    }

    public async Task<StudentDto?> FindStudentAsync(int id)
    {
        var student = await _students.GetByIdAsync(id, includeGroup: true);
        return student == null ? null : ToDto(student);
    }

    public async Task<List<StudentDto>> GetStudentsByGroupAsync(int groupId)
    {
        var list = await _students.GetByGroupAsync(groupId);

        return list.Select(ToDto).ToList();
    }

    public async Task<StudentDto> AddStudentAsync(StudentCreateRequest request)
    {
        var groupExists = await _groups.ExistsAsync(request.GroupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        if (await _students.LoginExistsAsync(request.Login))
        {
            throw new InvalidOperationException("Логин уже занят.");
        }

        var student = new Student
        {
            GroupId = request.GroupId,
            FullName = request.FullName,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Обучается" : request.Status,
            PreviousEducation = request.PreviousEducation,
            GraduationYear = request.GraduationYear,
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                string.IsNullOrWhiteSpace(request.Password) ? "student" : request.Password)
        };

        _students.Add(student);
        await _unitOfWork.SaveChangesAsync();

        await _students.LoadGroupAsync(student);
        return ToDto(student);
    }

    public async Task<StudentDto?> EditStudentAsync(StudentUpdateRequest request)
    {
        var student = await _students.GetByIdAsync(request.Id);
        if (student == null)
        {
            return null;
        }

        var groupExists = await _groups.ExistsAsync(request.GroupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        student.GroupId = request.GroupId;
        student.FullName = request.FullName;
        student.BirthDate = request.BirthDate;
        student.Phone = request.Phone;
        student.Email = request.Email;
        student.Address = request.Address;
        student.Status = request.Status;
        student.PreviousEducation = request.PreviousEducation;
        student.GraduationYear = request.GraduationYear;

        await _unitOfWork.SaveChangesAsync();
        await _students.LoadGroupAsync(student);
        return ToDto(student);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _students.GetByIdAsync(id);
        if (student == null)
        {
            return false;
        }

        _students.Remove(student);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<StudentDto?> ExpelStudentAsync(int id, string reason)
    {
        var student = await _students.GetByIdAsync(id, includeGroup: true);
        if (student == null)
        {
            return null;
        }

        student.Status = string.IsNullOrWhiteSpace(reason)
            ? "Отчислен"
            : $"Отчислен ({reason})";

        await _unitOfWork.SaveChangesAsync();
        return ToDto(student);
    }

    public async Task<StudentDto?> UpdatePersonalDataAsync(int studentId, PersonalDataRequest request)
    {
        var student = await _students.GetByIdAsync(studentId, includeGroup: true);
        if (student == null)
        {
            return null;
        }

        student.FullName = request.FullName;
        student.BirthDate = request.BirthDate;
        student.Phone = request.Phone;
        student.Email = request.Email;
        student.Address = request.Address;
        student.PreviousEducation = request.PreviousEducation;
        student.GraduationYear = request.GraduationYear;

        await _unitOfWork.SaveChangesAsync();
        return ToDto(student);
    }

    private static StudentDto ToDto(Student s) => new()
    {
        Id = s.Id,
        GroupId = s.GroupId,
        GroupName = s.Group?.Name,
        FullName = s.FullName,
        BirthDate = s.BirthDate,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        Status = s.Status,
        PreviousEducation = s.PreviousEducation,
        GraduationYear = s.GraduationYear,
        Login = s.Login
    };
}
