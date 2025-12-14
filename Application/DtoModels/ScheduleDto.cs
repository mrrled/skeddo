namespace Application.DtoModels;

public class ScheduleDto : IComparable<ScheduleDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<LessonDto> Lessons { get; set; } = new();
    public List<LessonDraftDto> LessonDrafts { get; set; } = new();
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(ScheduleDto? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }
}