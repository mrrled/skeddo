using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Schedules")]
public class ScheduleDbo
{
    public int Id { get; set; }
    public int ScheduleGroupId { get; set; }
    public string Name { get; set; }
    public ScheduleGroupDbo ScheduleGroup { get; set; }
    public ICollection<LessonDbo> Lessons { get; set; } 
    public ICollection<LessonNumberDbo> LessonNumbers { get; set; }
}