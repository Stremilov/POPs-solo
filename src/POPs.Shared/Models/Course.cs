namespace POPs.Shared.Models;

public class Course
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }

    public List<Group> Groups { get; set; } = new();
}
