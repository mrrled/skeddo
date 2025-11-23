using Domain;
using Domain.Models;
using Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public static class ExportGenerator
{
    public static void GeneratePdf(IScheduleRepository scheduleRepository, int scheduleId)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var generator = GetExportDocument(scheduleRepository, scheduleId).Result;
        generator.GeneratePdf(@"G:\skeddo\newUI\test.pdf");
    }

    public static void GenerateExcel(IScheduleRepository scheduleRepository, int scheduleId)
    {
        var generator = GetExportDocument(scheduleRepository, scheduleId).Result;
        generator.CreateExcelReport(@"G:\skeddo\newUI\schedule.xlsx");
    }

    private static async Task<ExportDocument> GetExportDocument(IScheduleRepository scheduleRepository, int scheduleId)
    {
        var lessons = await scheduleRepository.GetLessonsByScheduleIdAsync(scheduleId);
        var table = lessons.ToTable();
        var lessonNumbers = await scheduleRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        var studyGroups = await scheduleRepository.GetStudyGroupListAsync();
        return new ExportDocument(table, studyGroups, lessonNumbers);
    }

    private static Dictionary<LessonNumber, Dictionary<StudyGroup, List<Lesson>>> ToTable(this List<Lesson> lessons)
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