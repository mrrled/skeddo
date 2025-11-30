namespace Application.DtoModels;

public class StudyGroupDto
{
    public string Name { get; set; } = string.Empty; 
    
    public override string ToString() => Name;
}