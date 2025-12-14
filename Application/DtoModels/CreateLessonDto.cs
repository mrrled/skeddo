using Domain.Models;

namespace Application.DtoModels;

public class CreateLessonDto
{
    public SchoolSubjectDto? SchoolSubject { get; set; }
    public LessonNumberDto? LessonNumber { get; set; }
    public TeacherDto? Teacher { get; set; }
    public StudyGroupDto? StudyGroup { get; set; }
    public StudySubgroupDto? StudySubgroup { get; set; }
    public ClassroomDto? Classroom { get; set; }
    public string? Comment { get; set; }
    public WarningType WarningType { get; set; }
}