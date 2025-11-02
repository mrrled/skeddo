using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("ScheduleGroups")]
public class ScheduleGroupDbo
{
    public int Id { get; set; }
    public ICollection<ScheduleDbo> Schedules { get; set; } = new List<ScheduleDbo>();
    public ICollection<TeacherDbo> Teachers { get; set; } = new List<TeacherDbo>();
    public ICollection<ClassroomDbo> Classrooms { get; set; } = new List<ClassroomDbo>();
    public ICollection<StudyGroupDbo> StudyGroups { get; set; } = new List<StudyGroupDbo>();
    public ICollection<SchoolSubjectDbo> SchoolSubjects { get; set; } = new List<SchoolSubjectDbo>();
}