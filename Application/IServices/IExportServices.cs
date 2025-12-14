namespace Application.IServices;

public interface IExportServices
{
    public Task GeneratePdfAsync(Guid scheduleId);

    public Task GenerateExcelAsync(Guid scheduleId);
}