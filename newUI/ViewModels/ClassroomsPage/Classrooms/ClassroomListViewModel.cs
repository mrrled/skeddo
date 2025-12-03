using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.ClassroomsPage.Classrooms;

namespace newUI.ViewModels.ClassroomsPage.Classrooms;

public class ClassroomListViewModel : ViewModelBase
{
    private AvaloniaList<ClassroomDto> classroom = new();

    private IServiceScopeFactory scopeFactory;
    private IWindowManager windowManager;

    public double Width { get; set; }

    public AvaloniaList<ClassroomDto> Classrooms
    {
        get => classroom;
        set => SetProperty(ref classroom, value);
    }

    public ICommand CreateClassroomCommand { get; }
    public ICommand LoadClassroomsCommand { get; }
    public ICommand HideClassroomsCommand { get; }

    public ClassroomListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        CreateClassroomCommand = new RelayCommandAsync(CreateClassroom);
        LoadClassroomsCommand = new RelayCommandAsync(LoadClassrooms);
        HideClassroomsCommand = new RelayCommandAsync(HideClassrooms);
    }

    private Task CreateClassroom()
    {
        windowManager.Show<ClassroomCreationViewModel>();
        return Task.CompletedTask;
    }

    private Task HideClassrooms()
    {
        Classrooms.Clear();
        return Task.CompletedTask;
    }

    private Task LoadClassrooms()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClassroomServices>();
        var fetchedItems = service.FetchClassroomsFromBackendAsync().Result;
        var newClassroomsList = new AvaloniaList<ClassroomDto>(fetchedItems);
        Classrooms = newClassroomsList;
        return Task.CompletedTask;
    }
}