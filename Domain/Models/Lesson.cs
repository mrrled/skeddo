namespace Domain.Models;

public class Lesson(
    int id,
    SchoolSubject subject,
    TimeSlot timeSlot,
    Teacher teacher,
    StudyGroup studyGroup,
    Classroom classroom,
    string comment,
    bool isConflict)
{
    public int Id { get; } = id;
    public SchoolSubject Subject { get; } = subject;
    public TimeSlot TimeSlot { get; } = timeSlot;
    public Teacher Teacher { get; } = teacher;
    public StudyGroup StudyGroup { get; } = studyGroup;
    public Classroom Classroom { get; } = classroom;
    public string Comment { get; } = comment;
    public bool IsConflict { get; } = isConflict;
}