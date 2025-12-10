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
using newUI.Services;
using newUI.ViewModels;
using newUI.ViewModels.ClassroomsPage.ClassroomCreation;
using newUI.ViewModels.ClassroomsPage.ClassroomList;
using newUI.ViewModels.MainPage.ScheduleEditor;
using newUI.ViewModels.MainPage.ScheduleList;
using newUI.ViewModels.Navigation;
using newUI.ViewModels.SchedulePage.Schedule;
using newUI.ViewModels.SchedulePage.Lessons;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectCreation;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;
using newUI.ViewModels.TeachersPage.TeacherEditor;
using newUI.ViewModels.TeachersPage.TeacherList;
using newUI.Views.MainWindow;
using newUI.Views.SchedulePage.ScheduleTable;
using newUI.Views.SchedulePage.ScheduleWindow;
using newUI.Views.TeachersPage.TeacherList;
using newUI.Views.SchoolSubjectsPage.SchoolSubjectCreation;
using newUI.Views.SchoolSubjectsPage.SchoolSubjectList;
using newUI.Views.ClassroomsPage.ClassroomCreation;
using newUI.Views.ClassroomsPage.ClassroomList;
using newUI.Views.MainPage.ScheduleEditor;
using newUI.Views.MainPage.ScheduleList;
using newUI.Views.SchedulePage.LessonCreationWindow;
using newUI.Views.TeachersPage.TeacherEditor;

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
        
        RegisterServices(services);
        RegisterRepositories(services);
        services.AddScoped<ILessonFactory, LessonFactory>();
        
        services.AddDbContext<ScheduleDbContext>(options =>
        {
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
        });
        
        RegisterFront(services);
        
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

    private void RegisterFront(ServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainWindow>();
        
        services.AddSingleton<IWindowManager, WindowManager>();
        
        services.AddTransient<ScheduleListView>();
        services.AddTransient<ScheduleListViewModel>();
        services.AddTransient<ScheduleEditorViewModel>();
        
        services.AddTransient<LessonCreationViewModel>();
        services.AddTransient<LessonCreationWindow>();
        
        services.AddTransient<ScheduleViewModel>();
        services.AddTransient<ScheduleWindow>();
        
        services.AddTransient<LessonTableView>();
        services.AddTransient<LessonTableViewModel>();
        
        services.AddTransient<TeacherListView>();
        services.AddTransient<TeacherListViewModel>();
        services.AddTransient<TeacherEditorViewModel>();
        
        services.AddTransient<SchoolSubjectListView>();
        services.AddTransient<SchoolSubjectListViewModel>();
        services.AddTransient<SchoolSubjectCreationViewModel>();
        
        services.AddTransient<ClassroomListView>();
        services.AddTransient<ClassroomListViewModel>();
        services.AddTransient<ClassroomCreationViewModel>();
        
        services.AddTransient<LessonCardViewModel>();

        services.AddSingleton<NavigationService>();
        services.AddTransient<NavigationBarViewModel>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        RegisterViewMappings();
    }

    private static void RegisterServices(ServiceCollection services)
    {
        services.AddScoped<IClassroomServices, ClassroomServices>();
        services.AddScoped<ILessonNumberServices, LessonNumberServices>();
        services.AddScoped<ILessonServices, LessonServices>();
        services.AddScoped<IScheduleServices, ScheduleServices>();
        services.AddScoped<ISchoolSubjectServices, SchoolSubjectServices>();
        services.AddScoped<IStudyGroupServices, StudyGroupServices>();
        services.AddScoped<ITeacherServices, TeacherServices>();
        services.AddScoped<ILessonDraftServices, LessonDraftServices>();
        services.AddScoped<IStudySubgroupService, StudySubgroupService>();
    }

    private void RegisterRepositories(ServiceCollection services)
    {
        services.AddScoped<IClassroomRepository, ClassroomRepository>();
        services.AddScoped<ILessonNumberRepository, LessonNumberRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<ISchoolSubjectRepository, SchoolSubjectRepository>();
        services.AddScoped<IStudyGroupRepository, StudyGroupRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<ILessonDraftRepository, LessonDraftRepository>();
        services.AddScoped<IStudySubgroupRepository, StudySubgroupRepository>();
    }
    
    private static void RegisterViewMappings()
    {
        ViewMappingService.RegisterWindow<MainViewModel, MainWindow>();
        
        ViewMappingService.RegisterWindow<ScheduleEditorViewModel, ScheduleEditorWindow>();
        ViewMappingService.RegisterUserControl<ScheduleListViewModel, ScheduleListView>();
        
        ViewMappingService.RegisterWindow<LessonCreationViewModel, LessonCreationWindow>();
        ViewMappingService.RegisterUserControl<ScheduleViewModel, ScheduleWindow>();
        
        ViewMappingService.RegisterWindow<TeacherEditorViewModel, TeacherEditorWindow>();
        ViewMappingService.RegisterUserControl<TeacherListViewModel, TeacherListView>();
        
        ViewMappingService.RegisterWindow<SchoolSubjectCreationViewModel, SchoolSubjectCreationWindow>();
        ViewMappingService.RegisterUserControl<SchoolSubjectListViewModel, SchoolSubjectListView>();
        
        ViewMappingService.RegisterWindow<ClassroomCreationViewModel, ClassroomCreationWindow>();
        ViewMappingService.RegisterUserControl<ClassroomListViewModel, ClassroomListView>();
    }
}