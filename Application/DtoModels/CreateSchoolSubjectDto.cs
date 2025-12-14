namespace Application.DtoModels;

public class CreateSchoolSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public override string ToString() => Name;
}