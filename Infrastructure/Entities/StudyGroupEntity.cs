using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("StudyGroups")]
public class StudyGroupEntity
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}