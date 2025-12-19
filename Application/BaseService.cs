using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application;

public abstract class BaseService(IUnitOfWork unitOfWork, ILogger logger)
{
    protected readonly IUnitOfWork UnitOfWork = unitOfWork;
    protected readonly ILogger Logger = logger;

    protected async Task<Result> TrySaveChangesAsync(string errorMessage)
    {
        try
        {
            await UnitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, "DB Update Error: {Msg}", errorMessage);
            return Result.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Unexpected Error");
            return Result.Failure("Внутренняя ошибка сервера.");
        }
    }

    protected async Task<Result<T>> TrySaveChangesAsync<T>(T resultValue, string errorMessage)
    {
        try
        {
            await UnitOfWork.SaveChangesAsync();
            return Result<T>.Success(resultValue);
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, "DB Update Error: {Msg}", errorMessage);
            return Result<T>.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Unexpected Error");
            return Result<T>.Failure("Внутренняя ошибка сервера.");
        }
    }

    protected async Task<Result> ExecuteRepositoryTask(Func<Task> task, string errorMessage)
    {
        try
        {
            await task();
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Entity not found");
            return Result.Failure("Запись не найдена в базе данных.");
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, "DB Update Error: {Msg}", errorMessage);
            return Result.Failure(errorMessage);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, "Infrastructure error");
            return Result.Failure(errorMessage);
        }
    }
}