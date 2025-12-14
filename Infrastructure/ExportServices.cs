using Application.IServices;

namespace Infrastructure;

public class ExportServices(ExportGenerator exportGenerator) : IExportServices
{
    public async Task GeneratePdfAsync(int scheduleId) => await exportGenerator.GeneratePdfAsync(scheduleId);
    
    public async Task GenerateExcelAsync(int scheduleId) => await exportGenerator.GenerateExcelAsync(scheduleId);
}