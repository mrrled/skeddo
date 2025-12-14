using Domain.Models;

namespace Application.DtoModels;

public class LessonDraftDto
{
    public Guid Id { get; set; }
    public int ScheduleId { get; set; }
    public SchoolSubjectDto? SchoolSubject { get; set; }
    public LessonNumberDto? LessonNumber { get; set; }
    public TeacherDto? Teacher { get; set; }
    public StudyGroupDto? StudyGroup { get; set; }
    public ClassroomDto? Classroom { get; set; }
    public string? Comment { get; set; }

    public LessonDto ToLessonDto()
    {
        return new LessonDto
        {
            SchoolSubject = this.SchoolSubject,
            LessonNumber = this.LessonNumber,
            Teacher = this.Teacher,
            StudyGroup = this.StudyGroup,
            Comment = this.Comment
        };
    }

    public bool Equals(LessonDraftDto? other)
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
        return Equals((LessonDraftDto)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}