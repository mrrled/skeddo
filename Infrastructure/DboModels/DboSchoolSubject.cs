using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DboModels;

[Table("SchoolSubjects")]
public class DboSchoolSubject
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
}