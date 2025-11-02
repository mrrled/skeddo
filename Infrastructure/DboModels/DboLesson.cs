using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Lessons")]
public class DboLesson
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int TeacherId { get; set; }
    public int LessonNumber { get; set; }
    public string StudyGroup { get; set; } = string.Empty;
    public string Classroom { get; set; } = string.Empty;
    public string SchoolSubject { get; set; } = string.Empty;
}