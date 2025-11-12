namespace Application.DtoModels;

public class ClassroomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    
    public override string ToString() => Name;
}