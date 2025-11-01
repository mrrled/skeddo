using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("SubjectTeacher")]
public class TeacherSubjectDbo
{
    public int TeacherId { get; set; }
    public required string SchoolSubject { get; set; }
    public int GroupId { get; set; }
}