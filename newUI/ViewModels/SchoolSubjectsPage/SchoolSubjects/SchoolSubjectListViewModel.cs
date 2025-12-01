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

    public ICommand CreateTeacherCommand { get; }
    public ICommand LoadTeachersCommand { get; }
    public ICommand HideTeachersCommand { get; }

    public SchoolSubjectListViewModel(ISchoolSubjectServices service, IWindowManager windowManager)
    {
        this.service = service;
        this.windowManager = windowManager;
        CreateTeacherCommand = new RelayCommandAsync(CreateTeacher);
        LoadTeachersCommand = new RelayCommandAsync(LoadTeachers);
        HideTeachersCommand = new RelayCommandAsync(HideTeachers);
    }

    private Task CreateTeacher()
    {
        // windowManager.Show<TeacherCreationViewModel>();
        return Task.CompletedTask;
    }

    private Task HideTeachers()
    {
        SchoolSubjects.Clear();
        return Task.CompletedTask;
    }

    private Task LoadTeachers()
    {
        var fetchedItems = service.FetchSchoolSubjectsFromBackendAsync().Result;
        var newSchoolSubjectsList = new AvaloniaList<SchoolSubjectDto>(fetchedItems);
        SchoolSubjects = newSchoolSubjectsList;
        return Task.CompletedTask;
    }
}