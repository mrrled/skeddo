namespace Domain.Models;

public class Teacher(
    int Id,
    string Name,
    string Surname,
    string patronymic,
    List<SchoolSubject> Specializations,
    List<StudyGroup> StudyGroups,
    string? Description = null)
{
    public string Name { get; } = Name;
    public string Surname { get; } = Surname;
    public string Patronymic { get; } = patronymic;
    public List<SchoolSubject> Specializations { get; } = Specializations;
}