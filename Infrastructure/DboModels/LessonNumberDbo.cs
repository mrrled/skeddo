using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("LessonNumbers")]
public class LessonNumberDbo
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public string Time { get; set; } = string.Empty;
    public ScheduleDbo Schedule { get; set; }
}