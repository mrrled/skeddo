using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DboModels;

[Table("Classrooms")]
public class DboClassroom
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}