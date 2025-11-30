namespace Application.DtoModels;

public class TeacherDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public List<string> SchoolSubjects { get; set; } = new();
    public List<string> StudyGroups { get; set; } = new();
}