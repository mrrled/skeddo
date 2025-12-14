using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("LessonNumbers")]
public class LessonNumberDbo
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public int Number { get; set; }
    public string Time { get; set; } = string.Empty;
    public ScheduleDbo Schedule { get; set; }
}