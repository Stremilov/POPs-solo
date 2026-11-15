namespace POPs.Shared.Models;

public class Student
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "Обучается";
    public string PreviousEducation { get; set; } = string.Empty;
    public int GraduationYear { get; set; }

    public string Login { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Group? Group { get; set; }
}
