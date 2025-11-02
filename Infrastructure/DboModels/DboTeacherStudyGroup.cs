using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.DboModels;

[Table("StudyGroupTeacher")]
public class DboTeacherStudyGroup
{
    public int TeacherId { get; set; }
    public string StudyGroup { get; set; }
    public int GroupId { get; set; }
}