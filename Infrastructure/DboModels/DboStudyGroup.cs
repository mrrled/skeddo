using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DboModels;

[Table("StudyGroups")]
public class DboStudyGroup
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}