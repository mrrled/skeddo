namespace Application.DtoModels;

public class TeacherDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Patronymic { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public List<SchoolSubjectDto> SchoolSubjects { get; set; } = new();
    public List<StudyGroupDto> StudyGroups { get; set; } = new();
    public string FullName =>  $"{Surname} {Name.First()}." + (string.IsNullOrEmpty(Patronymic) ? string.Empty : $"{Patronymic.First()}.");
}