namespace Application.DtoModels;

public class SchoolSubjectDto
{
    public string Name { get; set; } = string.Empty;
    
    public override string ToString() => Name;
}