using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Entities;

[PrimaryKey(nameof(Id))]
[Table("Teachers")]
public class TeacherEntity
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Description { get; set; }
}