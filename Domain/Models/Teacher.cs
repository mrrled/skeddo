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

    public Result SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Имя учителя не может быть пустым.");
        Name = name;
        return Result.Success();
    }

    public Result SetSurname(string? surname)
    {
        if (string.IsNullOrWhiteSpace(surname))
            return Result.Failure("Фамилия учителя не может быть пустой.");
        Surname = surname;
        return Result.Success();
    }

    public Result SetPatronymic(string? patronymic)
    {
        if (string.IsNullOrWhiteSpace(patronymic))
            return Result.Failure("Отчество учителя не может быть пустым.");
        Patronymic = patronymic;
        return  Result.Success();
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
    
    public static Result<Teacher?> CreateTeacher(Guid teacherId,
        string? name, string? surname, string? patronymic,
        List<SchoolSubject> schoolSubjects, List<StudyGroup> studyGroups)
    {
        if (name is null)
            return Result<Teacher>.Failure("Имя учителя не может быть пустым.");
        if (surname is null)
            return Result<Teacher>.Failure("Фамилия учителя не может быть пустой");
        if (patronymic is null)
            return Result<Teacher>.Failure("Отчество учителя не может быть пустым");
        return Result<Teacher>.Success(new Teacher(teacherId, name, surname, patronymic, schoolSubjects, studyGroups));
    }
}