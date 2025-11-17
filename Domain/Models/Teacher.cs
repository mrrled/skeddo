namespace Domain.Models;

public class Teacher(
    int id,
    string name,
    string surname,
    string patronymic,
    List<SchoolSubject> schoolSubjects,
    List<StudyGroup> studyGroups) : Entity<int>(id)
{
    public string Name { get; set; } = name;
    public string Surname { get; set; } = surname;
    public string Patronymic { get; set; } = patronymic;
    public List<SchoolSubject> SchoolSubjects { get; set; } = schoolSubjects;
    public List<StudyGroup> StudyGroups { get; set; } = studyGroups;
}