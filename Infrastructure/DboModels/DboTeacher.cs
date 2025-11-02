using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("Teachers")]
public class DboTeacher
{
    public int Id { get; set; }
    public int ScheduleGroupId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Patronymic { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScheduleGroupDbo ScheduleGroup { get; set; }
    public ICollection<LessonDbo> Lessons { get; set; } = new List<LessonDbo>();
    public ICollection<SchoolSubjectDbo> SchoolSubjects { get; set; } = new List<SchoolSubjectDbo>();
    public ICollection<StudyGroupDbo> StudyGroups { get; set; } = new List<StudyGroupDbo>();
}