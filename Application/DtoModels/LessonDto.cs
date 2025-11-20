using Domain.Models;

namespace Application.DtoModels;

public class LessonDto
{
    public int Id { get; set; }
    public SchoolSubjectDto? Subject { get; set; }
    public LessonNumberDto? LessonNumber { get; set; }
    public TeacherDto? Teacher { get; set; }
    public StudyGroupDto? StudyGroup { get; set; }
    public ClassroomDto? Classroom { get; set; }
    public string? Comment { get; set; }
    public WarningType WarningType { get; set; }
}