namespace Application.DtoModels;

public class DtoTeacher
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string? Specialty { get; set; }
}