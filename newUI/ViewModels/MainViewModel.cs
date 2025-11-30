using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.Services;

namespace newUI.ViewModels;

public class MainViewModel : ViewModelBase
{
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