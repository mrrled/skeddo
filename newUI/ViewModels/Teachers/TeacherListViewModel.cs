using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using newUI.Services;

namespace newUI.ViewModels.Teachers;

public class TeacherListViewModel : ViewModelBase
{
    private AvaloniaList<TeacherDto> teachers = new(); 
        
    private ITeacherServices service;
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

    public TeacherListViewModel(ITeacherServices service, IWindowManager windowManager)
    {
        this.service = service;
        this.windowManager = windowManager;
        CreateTeacherCommand = new RelayCommandAsync(CreateTeacher);
        LoadTeachersCommand = new RelayCommandAsync(LoadTeachers);
        HideTeachersCommand = new RelayCommandAsync(HideTeachers);
    }

    private Task CreateTeacher()
    {
        windowManager.Show<TeacherCreationViewModel>();
        return Task.CompletedTask;
    }
        
    private Task HideTeachers()
    {
        Teachers.Clear();
        return Task.CompletedTask;
    }
        
    private Task LoadTeachers()
    {
        var fetchedItems =  service.FetchTeachersFromBackendAsync().Result;
        var newTeachersList = new AvaloniaList<TeacherDto>(fetchedItems);
        Teachers = newTeachersList;
        return Task.CompletedTask;
    }
}