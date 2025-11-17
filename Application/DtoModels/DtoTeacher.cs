namespace Application.DtoModels;

public class DtoTeacher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public List<string> Specialty { get; set; }
    public List<string> StudyGroups { get; set; }
}