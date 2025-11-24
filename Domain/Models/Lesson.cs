namespace Domain.Models;

public class Lesson(int id,
    SchoolSubject? subject,
    LessonNumber? lessonNumber,
    Teacher? teacher,
    StudyGroup? studyGroup,
    Classroom? classroom,
    string comment = "",
    WarningType warningType = WarningType.Normal
    ) : Entity<int>(id)
{
    public SchoolSubject? Subject { get; private set; } = subject;
    public LessonNumber? LessonNumber { get; private set; } = lessonNumber;
    public Teacher? Teacher { get; private set; } = teacher;
    public StudyGroup? StudyGroup { get; private set; } = studyGroup;
    public Classroom? Classroom { get; private set; } = classroom;
    public string? Comment { get; private set; } = comment;
    public WarningType WarningType { get; private set; } = warningType;
    //хранить список с кем конфликтует?
    internal void SetWarningType(WarningType warningType)
    {
        WarningType = warningType;
    }
    
    public Lesson Update(SchoolSubject? subject, LessonNumber? lessonNumber, Teacher? teacher, StudyGroup? studyGroup,
        Classroom? classroom, string? comment = null)
    {
        Subject = subject;
        LessonNumber = lessonNumber;
        Teacher = teacher;
        StudyGroup = studyGroup;
        Classroom = classroom;
        Comment = comment;
        return this;
    }
}