using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("SchoolSubjects")]
public class SchoolSubjectEntity
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}