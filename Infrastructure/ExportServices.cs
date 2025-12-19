using Application.IServices;

namespace Infrastructure;

public class ExportServices(ExportGenerator exportGenerator) : IExportServices
{
    public async Task GeneratePdfAsync(Guid scheduleId, Stream fileStream) => await exportGenerator.GeneratePdfAsync(scheduleId, fileStream);
    
    public async Task GenerateExcelAsync(Guid scheduleId, Stream fileStream) => await exportGenerator.GenerateExcelAsync(scheduleId, fileStream);
}