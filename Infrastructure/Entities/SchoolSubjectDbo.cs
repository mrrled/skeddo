using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("SchoolSubjects")]
public class SchoolSubjectDbo
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}