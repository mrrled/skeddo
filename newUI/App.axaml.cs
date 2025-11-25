using System;
using System.IO;
using Application;
using Application.IServices;
using Application.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Domain;
using Domain.IRepositories;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using newUI.ViewModels;
using newUI.Views;
using newUI.Views.MainWindow;

namespace newUI;

public partial class App : Avalonia.Application
{
    public static IServiceProvider Services { get; private set; }
    public static IConfiguration Configuration { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var services = new ServiceCollection();
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.SetMinimumLevel(LogLevel.Information);
        });

        services.AddScoped<IClassroomServices, ClassroomServices>();
        services.AddScoped<ILessonNumberServices, LessonNumberServices>();
        services.AddScoped<ILessonServices, LessonServices>();
        services.AddScoped<IScheduleServices, ScheduleServices>();
        services.AddScoped<ISchoolSubjectServices, SchoolSubjectServices>();
        services.AddScoped<IStudyGroupServices, StudyGroupServices>();
        services.AddScoped<ITeacherServices, TeacherServices>();
        
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<ILessonNumberRepository, LessonNumberRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<ISchoolSubjectRepository, SchoolSubjectRepository>();
        services.AddScoped<IStudyGroupRepository, StudyGroupRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        
        services.AddDbContext<ScheduleDbContext>(options =>
        {
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        Services = services.BuildServiceProvider();
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ScheduleDbContext>();
            context.Database.Migrate();
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}