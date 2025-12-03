using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

public class SchoolSubjectListViewModel : ViewModelBase
{
    private AvaloniaList<SchoolSubjectDto> schoolSubjects = new();

    private IServiceScopeFactory scopeFactory;
    private IWindowManager windowManager;

    public double Width { get; set; }

    public AvaloniaList<SchoolSubjectDto> SchoolSubjects
    {
        get => schoolSubjects;
        set => SetProperty(ref schoolSubjects, value);
    }

    public ICommand CreateSchoolSubjectCommand { get; }
    public ICommand LoadSchoolSubjectsCommand { get; }
    public ICommand HideSchoolSubjectsCommand { get; }

    public SchoolSubjectListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        CreateSchoolSubjectCommand = new RelayCommandAsync(CreateSchoolSubject);
        LoadSchoolSubjectsCommand = new RelayCommandAsync(LoadSchoolSubjects);
        HideSchoolSubjectsCommand = new RelayCommandAsync(HideSchoolSubjects);
    }

    private Task CreateSchoolSubject()
    {
        var scope = scopeFactory.CreateScope();
        var vm = scope.ServiceProvider.GetRequiredService<SchoolSubjectCreationViewModel>();
        windowManager.ShowWindow(vm);
        return Task.CompletedTask;
    }

    private Task HideSchoolSubjects()
    {
        SchoolSubjects.Clear();
        return Task.CompletedTask;
    }

    private Task LoadSchoolSubjects()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();
        var fetchedItems = service.FetchSchoolSubjectsFromBackendAsync().Result;
        var newSchoolSubjectsList = new AvaloniaList<SchoolSubjectDto>(fetchedItems);
        SchoolSubjects = newSchoolSubjectsList;
        return Task.CompletedTask;
    }
}