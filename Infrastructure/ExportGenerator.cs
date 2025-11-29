using Domain.IRepositories;
using Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

public static class ExportGenerator
{
    public static void GeneratePdf(
        ILessonRepository lessonRepository,
        ILessonNumberRepository lessonNumberRepository,
        IStudyGroupRepository studyGroupRepository,
        int scheduleId)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var generator = GetExportDocument(lessonRepository, lessonNumberRepository, studyGroupRepository, scheduleId)
            .Result;
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pathFile = Path.Combine(currentDirectory, "schedule.pdf");
        generator.GeneratePdf(pathFile);
    }

    public static void GenerateExcel(
        ILessonRepository lessonRepository,
        ILessonNumberRepository lessonNumberRepository,
        IStudyGroupRepository studyGroupRepository,
        int scheduleId)
    {
        var generator = GetExportDocument(lessonRepository,
                lessonNumberRepository,
                studyGroupRepository,
                scheduleId)
            .Result;
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pathFile = Path.Combine(currentDirectory, "schedule.xlsx");
        generator.CreateExcelReport(pathFile);
    }

    private static async Task<ExportDocument> GetExportDocument(
        ILessonRepository lessonRepository,
        ILessonNumberRepository lessonNumberRepository,
        IStudyGroupRepository studyGroupRepository,
        int scheduleId)
    {
        var lessons = await lessonRepository.GetLessonsByScheduleIdAsync(scheduleId);
        var table = lessons.ToTable();
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        var studyGroups = await studyGroupRepository.GetStudyGroupListAsync();
        return new ExportDocument(table, studyGroups, lessonNumbers);
    }
}