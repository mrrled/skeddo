namespace Domain.Models;

public class Lesson(int id,
    SchoolSubject subject,
    LessonNumber lessonNumber,
    Teacher teacher,
    StudyGroup studyGroup,
    Classroom classroom,
    string comment = "",
    bool isConflict = false
    ) : Entity<int>(id)
{
    public SchoolSubject Subject { get; } = subject;
    public LessonNumber LessonNumber { get; } = lessonNumber;
    public Teacher Teacher { get; } = teacher;
    public StudyGroup StudyGroup { get; } = studyGroup;
    public Classroom Classroom { get; } = classroom;
    public string Comment { get; } = comment;
    public bool IsConflict { get; } = isConflict;
}