using Application.IServices;

namespace Infrastructure;

public class ExportServices(ExportGenerator exportGenerator) : IExportServices
{
    public async Task GeneratePdfAsync(Guid scheduleId) => await exportGenerator.GeneratePdfAsync(scheduleId);
    
    public async Task GenerateExcelAsync(Guid scheduleId) => await exportGenerator.GenerateExcelAsync(scheduleId);
}