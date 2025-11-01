using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("StudyGroupTeacher")]
public class TeacherStudyGroupDbo
{
    public int TeacherId { get; set; }
    public string StudyGroup { get; set; }
    public int GroupId { get; set; }
}