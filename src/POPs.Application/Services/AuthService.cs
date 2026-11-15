using POPs.Application.Repositories;
using POPs.Shared.Dtos;

namespace POPs.Application.Services;

public class AuthService
{
    private readonly IAdministratorRepository _administrators;
    private readonly IStudentRepository _students;

    public AuthService(IAdministratorRepository administrators, IStudentRepository students)
    {
        _administrators = administrators;
        _students = students;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var login = request.Login.Trim();

        var admin = await _administrators.GetByLoginAsync(login);
        if (admin != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                return Fail("Неверный логин или пароль.");
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Вход выполнен",
                Role = "Admin",
                FullName = admin.FullName,
                Login = admin.Login,
                AdminId = admin.Id
            };
        }

        var student = await _students.GetByLoginAsync(login);
        if (student != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
            {
                return Fail("Неверный логин или пароль.");
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Вход выполнен",
                Role = "Student",
                FullName = student.FullName,
                Login = student.Login,
                StudentId = student.Id
            };
        }

        return Fail("Пользователь не найден.");
    }

    private static LoginResponse Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}
