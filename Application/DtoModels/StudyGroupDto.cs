namespace Application.DtoModels;

public class StudyGroupDto : IComparable<StudyGroupDto>
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StudySubgroupDto> StudySubgroups { get; set; } = new();

    public void UpdateSubgroups()
    {
        foreach (var studySubgroup in StudySubgroups)
        {
            studySubgroup.StudyGroup = this;
        }
    }
    
    public override string ToString() => Name;

    public int CompareTo(StudyGroupDto? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public int ColumnSpan => StudySubgroups?.Count > 0 ? StudySubgroups.Count : 1;
}