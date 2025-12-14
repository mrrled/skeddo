namespace Application.DtoModels;

public class CreateTeacherDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public List<SchoolSubjectDto> SchoolSubjects { get; set; } = new();
    public List<StudyGroupDto> StudyGroups { get; set; } = new();
}