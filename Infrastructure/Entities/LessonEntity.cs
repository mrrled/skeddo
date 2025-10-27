using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities;

[PrimaryKey(nameof(Id))]
[Table("Lessons")]
public class LessonEntity
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int TeacherId { get; set; }
    public int LessonNumber { get; set; }
    public string StudyGroup { get; set; } = string.Empty;
    public string Classroom { get; set; } = string.Empty;
    public string SchoolSubject { get; set; } = string.Empty;
}