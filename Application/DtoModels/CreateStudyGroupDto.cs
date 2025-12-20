namespace Application.DtoModels;

public class CreateStudyGroupDto
{
    public string Name { get; set; } = string.Empty;
    public Guid ScheduleId { get; set; }
    public List<StudySubgroupDto> StudySubgroups { get; set; } = new();
    public override string ToString() => Name;
}