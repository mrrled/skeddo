using Microsoft.IdentityModel.Tokens;

namespace Domain.Models;

public class Teacher(
    Guid id,
    string name,
    string surname,
    string patronymic,
    List<SchoolSubject> schoolSubjects,
    List<StudyGroup> studyGroups,
    string? description = null) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public string Surname { get; private set; } = surname;
    public string Patronymic { get; private set; } = patronymic;
    public string? Description { get; private set; } = description;
    public List<SchoolSubject> SchoolSubjects { get; private set; } = schoolSubjects;
    public List<StudyGroup> StudyGroups { get; private set; } = studyGroups;

    public void SetName(string? name)
    {
        if (name is null)
           throw new ArgumentNullException();
        Name = name;
    }

    public void SetSurname(string? surname)
    {
        if (surname is null)
            throw new ArgumentNullException();
        Surname = surname;
    }

    public void SetPatronymic(string? patronymic)
    {
        if (patronymic is null)
            throw new ArgumentNullException();
        Patronymic = patronymic;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetSchoolSubjects(List<SchoolSubject> schoolSubjects)
    {
        SchoolSubjects = schoolSubjects;
    }

    public void SetStudyGroups(List<StudyGroup> studyGroups)
    {
        StudyGroups = studyGroups;
    }
    
    public static Teacher CreateTeacher(Guid teacherId,
        string? name, string? surname, string? patronymic,
        List<SchoolSubject> schoolSubjects, List<StudyGroup> studyGroups)
    {
        if (name is null || surname is null || patronymic is null)
            throw new ArgumentNullException();
        return new Teacher(teacherId, name, surname, patronymic, schoolSubjects, studyGroups);
    }
    //setProperty..
}