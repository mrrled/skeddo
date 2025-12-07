namespace Application.DtoModels;

public class StudyGroupDto : IComparable<StudyGroupDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudySubgroupDto> StudySubgroups { get; set; } = new();
    
    public override string ToString() => Name;

    public int CompareTo(StudyGroupDto? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null ? 1 : Id.CompareTo(other.Id);
    }
}