using System.Windows.Input;
using Application.DtoModels;
using newUI.ViewModels.Lessons;

namespace newUI.ViewModels.Schedule;

public class ScheduleViewModel : ViewModelBase
{
    private DtoSchedule schedule;
    private LessonBufferViewModel buffer;
    private LessonTableViewModel table;

    public DtoSchedule Schedule
    {
        get => schedule;
        set => SetProperty(ref schedule, value);
    }
    
    public ICommand AddLessonCommand { get; set; }
    
    public 
}