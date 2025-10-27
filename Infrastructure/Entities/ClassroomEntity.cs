using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("Classrooms")]
public class ClassroomEntity
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}