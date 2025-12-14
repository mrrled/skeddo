namespace Application.DtoModels;

public class CreateStudyGroupDto
{
    public string Name { get; set; } = string.Empty;
    public List<StudySubgroupDto> StudySubgroups { get; set; } = new();
    public override string ToString() => Name;
}