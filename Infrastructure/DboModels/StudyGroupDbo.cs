using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("StudyGroups")]
public class StudyGroupDbo
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ScheduleDbo Schedule { get; set; }
    public ICollection<TeacherDbo> Teachers { get; set; } = new List<TeacherDbo>();
    public ICollection<StudySubgroupDbo> StudySubgroups { get; set; } = new List<StudySubgroupDbo>();
}