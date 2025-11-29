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
        generator.GeneratePdf(@"G:\skeddo\newUI\schedule.pdf");
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
}