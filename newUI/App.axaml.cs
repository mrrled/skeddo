using Application.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Infrastructure.Repositories;
using newUI.Views;

namespace newUI;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        teacherService = new TeacherService(new TeacherRepository());
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(teacherService);
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private ITeacherService teacherService;
}