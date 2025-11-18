namespace Domain.Models;

public class Schedule(
    int id,
    string Name,
    HashSet<Lesson> lessons
) : Entity<int>(id)
{
    private HashSet<Lesson> _lessons = lessons;
    public IReadOnlyCollection<Lesson> Lessons => _lessons;
    public string Name { get; } = Name;

    public static Teacher CreateTeacher(int teacherId,
        string? name, string? surname, string? patronymic,
        List<string> schoolSubjects, List<string> studyGroups)
    {
        if (name is null || surname is null || patronymic is null)
            throw new ArgumentNullException();
        var subjects = schoolSubjects.Select(CreateSchoolSubject).ToList();
        var groups = studyGroups.Select(CreateStudyGroup).ToList();
        return new Teacher(teacherId, name, surname, patronymic, subjects, groups);
    }

    public static SchoolSubject CreateSchoolSubject(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new SchoolSubject(name);
    }
    
    public static StudyGroup CreateStudyGroup(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudyGroup(name);
    }

    public static Classroom CreateClassroom(string? name, string? description)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new Classroom(name, description);
    }
    
    public static LessonNumber CreateLessonNumber(int lessonNumber, string? time)
    {
        if (lessonNumber < 0)
            throw new ArgumentException();
        return new LessonNumber(lessonNumber, time);
    }
    public Lesson AddLesson(int id, string? subject, int lessonNumber, Teacher? teacher, string? studyGroup,
        string? classroom, string? classroomDescription)
    {
        var warningType = WarningType.Normal;
        if (teacher is null || classroom is null)
            warningType = WarningType.Conflict;
        var lesson = new Lesson(id,
            subject is null ? null : CreateSchoolSubject(subject),
            new LessonNumber(lessonNumber, null), //а если не указан lessonNumber?
            teacher,
            studyGroup is null ? null : CreateStudyGroup(studyGroup),
            classroom is null ? null : CreateClassroom(classroom, classroomDescription),
            warningType: warningType);
        _lessons.Add(lesson);
        UpdateByLesson(lesson);
        return lesson;
    }

    private void UpdateByLesson(Lesson lesson) //возможно стоит возвращать список измененных уроков
    {
        var updatable = _lessons
            .Where(l => l.StudyGroup == lesson.StudyGroup || l.LessonNumber == lesson.LessonNumber)
            .ToList();
        foreach (var element in updatable.Where(l =>
                     l.Teacher?.Id == lesson.Teacher?.Id && l.Classroom == lesson.Classroom))
        {
            element.SetWarningType(WarningType.Warning);
            lesson.SetWarningType(WarningType.Warning);
        }

        UpdateByTeacherSpecialization(lesson);
    }

    private void UpdateByTeacherSpecialization(Lesson lesson)
    {
        if (lesson.Teacher is not null && !lesson.Teacher.SchoolSubjects.Contains(lesson.Subject))
            lesson.SetWarningType(WarningType.Warning);
    }
}