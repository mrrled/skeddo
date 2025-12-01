using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.Services;
using newUI.ViewModels.Navigation;
using newUI.ViewModels.TeachersPage.Teachers;

namespace newUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public NavigationBarViewModel NavigationBar { get; }

    private object? currentPage;

    public object? CurrentPage
    {
        get => currentPage;
        set => SetProperty(ref currentPage, value);
    }

    public MainViewModel(NavigationService nav, NavigationBarViewModel navigationBar)
    {
        NavigationBar = navigationBar;

        nav.CurrentViewModelChanged += vm => CurrentPage = vm;

        nav.Navigate<TeacherListViewModel>();
    }

    private AvaloniaList<ScheduleDto> scheduleList = new();
    private ScheduleDto currentSchedule; //заменить на ScheduleViewModel

    public AvaloniaList<ScheduleDto> SceduleList
    {
        get => scheduleList;
        set => SetProperty(ref scheduleList, value);
    }

    public ScheduleDto CurrentSchedule
    {
        get => currentSchedule;
        set => SetProperty(ref currentSchedule, value);
    }

    public ICommand SelectCurrentScheduleCommand { get; }
    public ICommand AddScheduleCommand { get; }
}