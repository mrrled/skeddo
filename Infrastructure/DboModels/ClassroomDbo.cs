using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Classrooms")]
public class ClassroomDbo
{
    public Guid Id { get; set; }
    public int ScheduleGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ScheduleGroupDbo ScheduleGroup { get; set; }
}