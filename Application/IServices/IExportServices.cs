namespace Application.IServices;

public interface IExportServices
{
    public Task GeneratePdfAsync(Guid scheduleId, Stream fileStream);

    public Task GenerateExcelAsync(Guid scheduleId, Stream fileStream);
}