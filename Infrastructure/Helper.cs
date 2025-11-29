using Domain.Models;

namespace Infrastructure;

public static class Helper
{
    public static Dictionary<LessonNumber, Dictionary<StudyGroup, List<Lesson>>> ToTable(this List<Lesson> lessons)
    {
        var table = new Dictionary<LessonNumber, Dictionary<StudyGroup, List<Lesson>>>();
        foreach (var lesson in lessons)
        {
            if (lesson.LessonNumber is null || lesson.StudyGroup is null)
                continue;
            if (table.TryGetValue(lesson.LessonNumber, out Dictionary<StudyGroup, List<Lesson>>? value))
            {
                if (value.TryGetValue(lesson.StudyGroup, out List<Lesson>? lessonsList))
                {
                    lessonsList.Add(lesson);
                }
                else
                {
                    value[lesson.StudyGroup] = [lesson];
                }
            }
            else
            {
                table[lesson.LessonNumber] = new Dictionary<StudyGroup, List<Lesson>>
                {
                    [lesson.StudyGroup] = [lesson]
                };
            }
        }
        return table;
    }
}