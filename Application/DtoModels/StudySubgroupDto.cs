namespace Application.DtoModels;

public class StudySubgroupDto
{
    public string Name { get; set; } = string.Empty; 
    public StudyGroupDto StudyGroup { get; set; } 
    
    public override string ToString() => Name;
}