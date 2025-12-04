namespace Application.DtoModels;

public class StudyGroupDto : IComparable<StudyGroupDto>
{
    public string Name { get; set; } = string.Empty; 
    
    public override string ToString() => Name;

    public int CompareTo(StudyGroupDto? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}