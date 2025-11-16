namespace Domain.Models;

public class Schedule(
    int id,
    HashSet<Lesson> lessons
) : Entity<int>(id)
{
    private HashSet<Lesson> _lessons = lessons;
    public IReadOnlyCollection<Lesson> Lessons => _lessons;

    public Lesson AddLesson(int id, string subject, int lessonNumber, Teacher? teacher, string? studyGroup,
        string? classroom)
    {
        var warningType = WarningType.Normal;
        if (teacher is null || classroom is null)
            warningType = WarningType.Conflict;
        var lesson = new Lesson(id,
            new SchoolSubject(subject),
            new LessonNumber(lessonNumber), //а если не указан?
            teacher,
            studyGroup is null ? null : new StudyGroup(studyGroup),
            classroom is null ? null : new Classroom(classroom),
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