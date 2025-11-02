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
    public int Id { get; } = Id;
    public SchoolSubject Subject { get; } = Subject;
    public LessonNumber LessonNumber { get; } = LessonNumber;
    public Teacher Teacher { get; } = Teacher;
    public StudyGroup StudyGroup { get; } = StudyGroup;
    public Classroom Classroom { get; } = Classroom;
    public string Comment { get; } = Comment;
    public bool IsConflict { get; } = IsConflict;
}