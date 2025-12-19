using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Lessons")]
public class LessonDbo
{
    public Guid Id { get; set; }
    public Guid LessonNumberId { get; set; }
    public Guid StudyGroupId { get; set; }
    public Guid? ClassroomId { get; set; }
    public Guid SchoolSubjectId { get; set; }
    public Guid? TeacherId { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid? StudySubgroupId { get; set; }
    public int WarningType { get; set; }
    public LessonNumberDbo LessonNumber { get; set; }
    public StudyGroupDbo StudyGroup { get; set; }
    public ClassroomDbo? Classroom { get; set; }
    public SchoolSubjectDbo SchoolSubject { get; set; }
    public TeacherDbo? Teacher { get; set; }
    public ScheduleDbo Schedule { get; set; }
    public StudySubgroupDbo StudySubgroup { get; set; }
}