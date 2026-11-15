namespace POPs.Shared.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public int CourseId { get; set; }

    public Course? Course { get; set; }
    public List<Student> Students { get; set; } = new();
}
