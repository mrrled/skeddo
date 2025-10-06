namespace Domain.Models;

public class Lesson
{
    public Guid Id { get; }
    public SchoolSubject Subject { get; }
    public TimeSlot TimeSlot { get; }
    public Teacher Teacher { get; }
    public StudyGroup StudyGroup { get; }
    public Classroom Classroom { get; }
    public string Comment { get; }
    public bool IsConflict { get; }
}