using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

[Table("StudyGroupTeacher")]
public class TeacherStudyGroupModel
{
    public int TeacherId { get; set; }
    public string StudyGroup { get; set; }
    public int GroupId { get; set; }
}