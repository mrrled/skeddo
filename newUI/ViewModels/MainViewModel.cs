using System.Threading.Tasks;
using System.Windows.Input;
using Application.Services;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.Services;
using newUI.ViewModels.Lessons;

namespace newUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private AvaloniaList<DtoSchedule> scheduleList = new();
    private DtoSchedule currentSchedule; //заменить на ScheduleViewModel
    
    public AvaloniaList<DtoSchedule> SceduleList
    {
        get => scheduleList;
        set => SetProperty(ref scheduleList, value);
    }

    public DtoSchedule CurrentSchedule
    {
        get => currentSchedule;
        set => SetProperty(ref currentSchedule, value);
    }
    
    public ICommand SelectCurrentScheduleCommand { get; }
    public ICommand AddScheduleCommand { get; }
    
}