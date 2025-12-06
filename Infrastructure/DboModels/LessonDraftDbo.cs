using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("LessonDrafts")]
public class LessonDraftDbo
{
    public int Id { get; set; }
    public int? LessonNumberId { get; set; }
    public int? StudyGroupId { get; set; }
    public int? ClassroomId { get; set; }
    public int? SchoolSubjectId { get; set; }
    public int? TeacherId { get; set; }
    public int? ScheduleId { get; set; }
    public LessonNumberDbo? LessonNumber { get; set; }
    public StudyGroupDbo? StudyGroup { get; set; }
    public ClassroomDbo? Classroom { get; set; }
    public required SchoolSubjectDbo SchoolSubject { get; set; }
    public TeacherDbo? Teacher { get; set; }
    public ScheduleDbo? Schedule { get; set; }
}