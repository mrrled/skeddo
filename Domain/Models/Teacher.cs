namespace Domain.Models;

public class Teacher(
    int Id,
    string Name,
    string Surname,
    string MiddleName,
    List<SchoolSubject> Specializations,
    List<StudyGroup> StudyGroups,
    string? Description = null)
{
}