namespace Application.IServices;

public interface IExportServices
{
    public Task GeneratePdfAsync(int scheduleId);
    public Task GenerateExcelAsync(int scheduleId);
}