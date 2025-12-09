using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DboModels;

[PrimaryKey(nameof(Id))]
[Table("StudySubgroups")]
public class StudySubgroupDbo
{
    public int Id { get; set; }
    public int StudyGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public StudyGroupDbo StudyGroup { get; set; }
}