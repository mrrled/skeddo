using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DboModels;

[Table("SubjectTeacher")]
public class DboTeacherSubject
{
    public int TeacherId { get; set; }
    public required string SchoolSubject { get; set; }
    public int GroupId { get; set; }
}