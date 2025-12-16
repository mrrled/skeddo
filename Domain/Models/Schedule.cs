namespace Domain.Models;

public class Schedule(
    Guid id,
    string name
) : Entity<Guid>(id)
{
    private HashSet<Lesson> _lessons = new();
    private HashSet<LessonDraft> _lessonDrafts = new();
    public IReadOnlyCollection<LessonDraft> LessonDrafts => _lessonDrafts;
    public IReadOnlyCollection<Lesson> Lessons => _lessons;
    public string Name { get; set; } = name;
    
    public List<Lesson> AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
        var editedLessons = UpdateByLesson(lesson);
        return editedLessons;
    }

    public void AddLessonDraft(LessonDraft lessonDraft)
    {
        _lessonDrafts.Add(lessonDraft);
    }
    public List<Lesson> EditLesson(Guid id, SchoolSubject? subject, LessonNumber? lessonNumber, Teacher? teacher, StudyGroup? studyGroup,
        Classroom? classroom, string? comment = null)
    {
        var lesson = Lessons.FirstOrDefault(x => x.Id == id);
        if (lesson is null)
            throw new ArgumentException($"There is no lesson with id {id}");
        lesson.SetSchoolSubject(subject);
        lesson.SetLessonNumber(lessonNumber);
        lesson.SetTeacher(teacher);
        lesson.SetStudyGroup(studyGroup);
        lesson.SetClassroom(classroom);
        lesson.SetComment(comment);
        var editedLessons = UpdateByLesson(lesson);
        editedLessons.Add(lesson);
        return editedLessons;
    }

    public static Schedule CreateSchedule(Guid id, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new Schedule(id, name);
    }

    public void SetName(string name)
    {
        Name = name;
    }

    private List<Lesson> UpdateByLesson(Lesson lesson)
    {
        var updatableLessons = _lessons
            .Where(l => (l.StudyGroup == lesson.StudyGroup) ^ (l.LessonNumber == lesson.LessonNumber))
            .ToList();
        foreach (var element in updatableLessons.Where(l =>
                     l.Teacher.Id == lesson.Teacher.Id || l.Classroom == lesson.Classroom))
        {
            element.SetWarningType(WarningType.Warning);
            lesson.SetWarningType(WarningType.Warning);
        }

        UpdateByTeacherSpecialization(lesson);
        return updatableLessons;
    }

    private void UpdateByTeacherSpecialization(Lesson lesson)
    {
        if (!lesson.Teacher.SchoolSubjects.Contains(lesson.SchoolSubject))
            lesson.SetWarningType(WarningType.Warning);
    }
}