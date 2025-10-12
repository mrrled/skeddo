using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        using (var scope = host.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            
            try
            {
                var context = serviceProvider.GetRequiredService<ScheduleDbContext>();
                context.Database.EnsureCreated();
                var scriptPath = "CreateDB.sql"; 

                var sqlScript = File.ReadAllText(scriptPath);
                context.Database.ExecuteSqlRaw(sqlScript);
                scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogInformation("Created DB script");
            }
            catch (Exception ex)
            {
                scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogError("Error initializing DB script");
            }
        }
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ScheduleDbContext>(options =>
                {
                    options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")); 
                });
            });
}