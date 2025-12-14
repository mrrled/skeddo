using Domain.IRepositories;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public class ExportGenerator(
    ILessonRepository lessonRepository,
    ILessonNumberRepository lessonNumberRepository,
    IStudyGroupRepository studyGroupRepository)
{
    public async Task GeneratePdfAsync(Guid scheduleId)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var generator = await GetExportDocumentAsync(scheduleId);

        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pathFile = Path.Combine(currentDirectory, "schedule.pdf");
        generator.GeneratePdf(pathFile);
    }

    public async Task GenerateExcelAsync(Guid scheduleId)
    {
        var generator = await GetExportDocumentAsync(scheduleId);
        var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var pathFile = Path.Combine(currentDirectory, "schedule.xlsx");
        generator.CreateExcelReport(pathFile);
    }

    private async Task<ExportDocument> GetExportDocumentAsync(Guid scheduleId)
    {
        var lessons = await lessonRepository.GetLessonsByScheduleIdAsync(scheduleId);
        var table = lessons.ToTable();
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        var studyGroups = await studyGroupRepository.GetStudyGroupListAsync(1);

        return new ExportDocument(table, studyGroups, lessonNumbers);
    }
}