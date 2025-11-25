using System.Collections.Generic;
using newUI.ViewModels.Lessons;

namespace newUI.ViewModels;

public class ScheduleRowViewModel : ViewModelBase
{
    public string TimeSlotDisplay { get; set; } 
    public int TimeSlotOrder { get; set; }
    public List<LessonCardViewModel?> Cells { get; set; } = new();
}