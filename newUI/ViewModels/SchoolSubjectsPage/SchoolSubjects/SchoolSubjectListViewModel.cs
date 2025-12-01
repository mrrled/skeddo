using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using newUI.Services;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

public class SchoolSubjectListViewModel : ViewModelBase
{
    private AvaloniaList<SchoolSubjectDto> schoolSubjects = new();

    private ISchoolSubjectServices service;
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

    public SchoolSubjectListViewModel(ISchoolSubjectServices service, IWindowManager windowManager)
    {
        this.service = service;
        this.windowManager = windowManager;
        CreateSchoolSubjectCommand = new RelayCommandAsync(CreateSchoolSubject);
        LoadSchoolSubjectsCommand = new RelayCommandAsync(LoadSchoolSubjects);
        HideSchoolSubjectsCommand = new RelayCommandAsync(HideSchoolSubjects);
    }

    private Task CreateSchoolSubject()
    {
        // windowManager.Show<SchoolSubjectCreationViewModel>();
        return Task.CompletedTask;
    }

    private Task HideSchoolSubjects()
    {
        SchoolSubjects.Clear();
        return Task.CompletedTask;
    }

    private Task LoadSchoolSubjects()
    {
        var fetchedItems = service.FetchSchoolSubjectsFromBackendAsync().Result;
        var newSchoolSubjectsList = new AvaloniaList<SchoolSubjectDto>(fetchedItems);
        SchoolSubjects = newSchoolSubjectsList;
        return Task.CompletedTask;
    }
}