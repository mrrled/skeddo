using Domain.Models;

namespace Application.DtoModels;

public class DtoLesson
{
    public int Id { get; set; }
    public DtoSchoolSubject? Subject { get; set; }
    public DtoLessonNumber? LessonNumber { get; set; }
    public DtoTeacher? Teacher { get; set; }
    public DtoStudyGroup? StudyGroup { get; set; }
    public DtoClassroom? Classroom { get; set; }
    public string? Comment { get; set; }
    public WarningType WarningType { get; set; }
}