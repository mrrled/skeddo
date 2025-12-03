using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;

namespace newUI.ViewModels.TeachersPage.Teachers;

public class TeacherListViewModel : ViewModelBase
{
    private AvaloniaList<TeacherDto> teachers = new(); 
    
    private readonly IServiceScopeFactory scopeFactory;
    private IWindowManager windowManager;
        
    public double Width { get; set; }
        
    public AvaloniaList<TeacherDto> Teachers
    {
        get => teachers;
        set => SetProperty(ref teachers, value);
    }
        
        
    public ICommand CreateTeacherCommand {get; }
    public ICommand LoadTeachersCommand { get; }
    public ICommand HideTeachersCommand { get; }

    public TeacherListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        CreateTeacherCommand = new RelayCommandAsync(CreateTeacher);
        LoadTeachersCommand = new RelayCommandAsync(LoadTeachers);
        HideTeachersCommand = new RelayCommandAsync(HideTeachers);
    }

    private Task CreateTeacher()
    {
        var scope = scopeFactory.CreateScope();
        var vm = scope.ServiceProvider.GetRequiredService<TeacherCreationViewModel>();
        windowManager.ShowWindow(vm);
        return Task.CompletedTask;
    }
        
    private Task HideTeachers()
    {
        Teachers.Clear();
        return Task.CompletedTask;
    }
        
    private Task LoadTeachers()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
        var fetchedItems =  service.FetchTeachersFromBackendAsync().Result;
        var newTeachersList = new AvaloniaList<TeacherDto>(fetchedItems);
        Teachers = newTeachersList;
        return Task.CompletedTask;
    }
}