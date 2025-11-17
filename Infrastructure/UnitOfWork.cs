using Application;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ScheduleDbContext _dbContext;

    public UnitOfWork(ScheduleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}