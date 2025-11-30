using Domain.Models;

namespace Application.DtoModels;

public class LessonDto : IEquatable<LessonDto>
{
    public int Id { get; set; }
    public SchoolSubjectDto? Subject { get; set; }
    public LessonNumberDto? LessonNumber { get; set; }
    public TeacherDto? Teacher { get; set; }
    public StudyGroupDto? StudyGroup { get; set; }
    public ClassroomDto? Classroom { get; set; }
    public string? Comment { get; set; }
    public WarningType WarningType { get; set; }

    public bool Equals(LessonDto? other)
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
        return Equals((LessonDto)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }
}