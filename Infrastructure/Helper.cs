using Domain.Models;

namespace Infrastructure;

public static class Helper
{
    public static Dictionary<(int num, Guid groupId, string? subgroupName), Lesson> ToTable(this List<Lesson> lessons)
    {
        var table = new  Dictionary<(int num, Guid groupId, string? subgroupName), Lesson>();
        foreach (var lesson in lessons)
        {
            if (lesson.StudySubgroup != null)
            {
                var key = (lesson.LessonNumber.Number, lesson.StudyGroup.Id, lesson.StudySubgroup.Name);
                table[key] = lesson;
            }
            else if (lesson.StudyGroup.StudySubgroups.Any())
            {
                foreach (var sub in lesson.StudyGroup.StudySubgroups)
                {
                    var key = (lesson.LessonNumber.Number, lesson.StudyGroup.Id, sub.Name);
                    table[key] = lesson;
                }
            }
            else 
            {
                var key = (lesson.LessonNumber.Number, lesson.StudyGroup.Id, (string?)null);
                table[key] = lesson;
            }
        }

        return table;
    }
}