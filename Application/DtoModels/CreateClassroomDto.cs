namespace Application.DtoModels;

public class CreateClassroomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public override string ToString() => Name;
}