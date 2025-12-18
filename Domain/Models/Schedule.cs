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
        var editedLessons = UpdateAllLessons();
        return editedLessons;
    }

    public void AddLessonDraft(LessonDraft lessonDraft)
    {
        _lessonDrafts.Add(lessonDraft);
    }

    public List<Lesson> EditLesson(Guid id, SchoolSubject? subject, LessonNumber? lessonNumber, Teacher? teacher,
        StudyGroup? studyGroup,
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
        var editedLessons = UpdateAllLessons();
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

    private List<Lesson> UpdateAllLessons()
    {
        var lessons = _lessons.ToList();
        var changedLessons = new HashSet<Lesson>();
        var originalStates = lessons.ToDictionary(l => l, l => l.WarningType);

        foreach (var lesson in lessons)
        {
            lesson.SetWarningType(WarningType.Normal);
        }

        for (var i = 0; i < lessons.Count; i++)
        {
            var lesson1 = lessons[i];

            for (var j = i + 1; j < lessons.Count; j++)
            {
                var lesson2 = lessons[j];
                if (lesson1.LessonNumber == lesson2.LessonNumber &&
                    lesson1.StudyGroup == lesson2.StudyGroup)
                {
                    lesson1.SetWarningType(WarningType.Conflict);
                    lesson2.SetWarningType(WarningType.Conflict);
                }
            }
        }

        for (var i = 0; i < lessons.Count; i++)
        {
            var lesson1 = lessons[i];
            for (var j = i + 1; j < lessons.Count; j++)
            {
                var lesson2 = lessons[j];

                if (lesson1.LessonNumber != lesson2.LessonNumber)
                    continue;

                var sameTeacher = lesson1.Teacher.Id == lesson2.Teacher.Id;
                var sameClassroom = lesson1.Classroom == lesson2.Classroom;

                if (sameTeacher ^ sameClassroom)
                {
                    if (lesson1.WarningType != WarningType.Conflict)
                        lesson1.SetWarningType(WarningType.Warning);

                    if (lesson2.WarningType != WarningType.Conflict)
                        lesson2.SetWarningType(WarningType.Warning);
                }
            }
        }

        foreach (var lesson in lessons)
        {
            var currentType = lesson.WarningType;
            var originalType = originalStates[lesson];

            if (currentType != originalType)
            {
                changedLessons.Add(lesson);
            }
        }
        return changedLessons.ToList();
    }
}