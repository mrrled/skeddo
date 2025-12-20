using Domain.IRepositories;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public class ExportGenerator(
    ILessonRepository lessonRepository,
    ILessonNumberRepository lessonNumberRepository,
    IStudyGroupRepository studyGroupRepository)
{
    public async Task GeneratePdfAsync(Guid scheduleId, Stream fileStream)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var generator = await GetExportDocumentAsync(scheduleId);
        generator.GeneratePdf(fileStream);
    }

    public async Task GenerateExcelAsync(Guid scheduleId, Stream fileStream)
    {
        var generator = await GetExportDocumentAsync(scheduleId);
        generator.CreateExcelReport(fileStream);
    }

    private async Task<ExportDocument> GetExportDocumentAsync(Guid scheduleId)
    {
        var lessons = await lessonRepository.GetLessonsByScheduleIdAsync(scheduleId);
        var table = lessons.ToTable();
        var lessonNumbers = await lessonNumberRepository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        var studyGroups = await studyGroupRepository.GetStudyGroupListByScheduleIdAsync(scheduleId);

        return new ExportDocument(table, studyGroups, lessonNumbers);
    }
}