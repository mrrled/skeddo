using System;
using System.IO;
using Application.Mapping;
using Application.Services;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Domain;
using Infrastructure;
using Infrastructure.DboMapping;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using newUI.ViewModels;
using newUI.Views;

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
        
        services.AddAutoMapper(_ => { }, typeof(ClassroomProfile));
        services.AddAutoMapper(_ => { }, typeof(LessonProfile));
        services.AddAutoMapper(_ => { }, typeof(ScheduleProfile));
        services.AddAutoMapper(_ => { }, typeof(SchoolSubjectProfile));
        services.AddAutoMapper(_ => { }, typeof(StudyGroupProfile));
        services.AddAutoMapper(_ => { }, typeof(TeacherProfile));
        services.AddAutoMapper(_ => { }, typeof(TimeSlotProfile));
        
        services.AddAutoMapper(_ => { }, typeof(DboClassroomProfile));
        services.AddAutoMapper(_ => { }, typeof(DboLessonProfile));
        services.AddAutoMapper(_ => { }, typeof(DboScheduleProfile));
        services.AddAutoMapper(_ => { }, typeof(DboSchoolSubjectProfile));
        services.AddAutoMapper(_ => { }, typeof(DboStudyGroupProfile));
        services.AddAutoMapper(_ => { }, typeof(DboTeacherProfile));

        services.AddScoped<IService, Service>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddDbContext<ScheduleDbContext>(options =>
        {
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();

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