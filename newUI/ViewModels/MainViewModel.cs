using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.Services;
using newUI.ViewModels.MainPage;
using newUI.ViewModels.MainPage.ScheduleList;
using newUI.ViewModels.Navigation;

namespace newUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase? currentPage;
    public ViewModelBase? CurrentPage
    {
        get => currentPage;
        set => SetProperty(ref currentPage, value);
    }

    public NavigationService Navigation { get; }
    public NavigationBarViewModel NavigationBar { get; }

    public MainViewModel(NavigationService navigation, NavigationBarViewModel navigationBar)
    {
        Navigation = navigation;
        NavigationBar = navigationBar;

        Navigation.CurrentViewModelChanged += () =>
        {
            CurrentPage = Navigation.CurrentViewModel;
        };
        
        Navigation.Navigate<ScheduleListViewModel>();
    }

    private AvaloniaList<ScheduleDto> scheduleList = new();
    private ScheduleDto currentSchedule; //заменить на ScheduleViewModel, когда будет готова

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