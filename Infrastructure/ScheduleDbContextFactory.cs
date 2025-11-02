using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class ScheduleDbContextFactory : IDesignTimeDbContextFactory<ScheduleDbContext>
{
    public ScheduleDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var startupProjectPathArg = args.FirstOrDefault(a => a.StartsWith("--startup-project"));

        if (!string.IsNullOrEmpty(startupProjectPathArg))
        {
            var pathParts = startupProjectPathArg.Split('=', 2);
            if (pathParts.Length > 1)
            {
                var potentialPath = pathParts[1].Trim('"').Trim('\'');
                if (Directory.Exists(potentialPath))
                    basePath = potentialPath;
            }
        }
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) 
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ScheduleDbContext>();
        optionsBuilder.UseSqlite(connectionString); 

        return new ScheduleDbContext(optionsBuilder.Options);
    }
}