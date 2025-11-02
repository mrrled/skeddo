namespace Domain.Models;

public class Lesson(int Id,
    SchoolSubject Subject,
    LessonNumber LessonNumber,
    Teacher Teacher,
    StudyGroup StudyGroup,
    Classroom Classroom,
    string Comment = "",
    bool IsConflict = false
    )
{
    public int Id { get; }
    public SchoolSubject Subject { get; }
    public LessonNumber LessonNumber { get; }
    public Teacher Teacher { get; }
    public StudyGroup StudyGroup { get; }
    public Classroom Classroom { get; }
    public string Comment { get; }
    public bool IsConflict { get; }
}