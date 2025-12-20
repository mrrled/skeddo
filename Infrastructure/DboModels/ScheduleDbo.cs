using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Schedules")]
public class ScheduleDbo
{
    public Guid Id { get; set; }
    public int ScheduleGroupId { get; set; }
    public string Name { get; set; }
    public ScheduleGroupDbo ScheduleGroup { get; set; }
    public ICollection<LessonDbo> Lessons { get; set; } 
    public ICollection<LessonDraftDbo> LessonDrafts { get; set; }
    public ICollection<LessonNumberDbo> LessonNumbers { get; set; }
    public ICollection<StudyGroupDbo> StudyGroups { get; set; } = new List<StudyGroupDbo>();
}