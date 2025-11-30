using Domain.Models;

namespace Application.DtoModels;

public class ScheduleDto : IEquatable<ScheduleDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();

    public bool Equals(ScheduleDto? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ScheduleDto)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}