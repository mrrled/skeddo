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
    
    public Lesson AddLesson(int id, string? subject, int lessonNumber, Teacher? teacher, string? studyGroup,
        string? classroom, string? classroomDescription)
    {
        var warningType = WarningType.Normal;
        if (teacher is null || classroom is null)
            warningType = WarningType.Conflict;
        var lesson = new Lesson(id,
            subject is null ? null : SchoolSubject.CreateSchoolSubject(subject),
            new LessonNumber(lessonNumber, null), //а если не указан lessonNumber?
            teacher,
            studyGroup is null ? null : StudyGroup.CreateStudyGroup(studyGroup),
            classroom is null ? null : Classroom.CreateClassroom(classroom, classroomDescription),
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