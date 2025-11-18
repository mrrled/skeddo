namespace Domain.Models;

public class Teacher(
    int id,
    string name,
    string surname,
    string patronymic,
    List<SchoolSubject> schoolSubjects,
    List<StudyGroup> studyGroups,
    string? description = null) : Entity<int>(id)
{
    public string Name { get; private set; } = name;
    public string Surname { get; private set; } = surname;
    public string Patronymic { get; private set; } = patronymic;
    public string? Description { get; private set; } = description;
    public List<SchoolSubject> SchoolSubjects { get; private set; } = schoolSubjects;
    public List<StudyGroup> StudyGroups { get; private set; } = studyGroups;

    public Teacher Update(string? name, string? surname, string? patronymic, List<string> schoolSubjects,
        List<string> studyGroups, string? description = null)
    {
        if (name is null || surname is null || patronymic is null)
            throw new ArgumentNullException();
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        SchoolSubjects = schoolSubjects.Select(Schedule.CreateSchoolSubject).ToList();
        StudyGroups = studyGroups.Select(Schedule.CreateStudyGroup).ToList();
        Description = description;
        return this;
    }
}