using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Application.DtoModels;
using Application.Services;
using Avalonia.Collections;

namespace newUI.ViewModels.Schedule;

public class ScheduleViewModel : ViewModelBase
{
    private readonly ScheduleServices scheduleServices;
    private StudyGroupServices studyGroupServices;
    private LessonNumberServices lessonNumberServices;
    private LessonServices lessonServices;
    
    private AvaloniaList<ScheduleDto> scheduleList;
    private AvaloniaDictionary<ScheduleDto, LessonTableViewModel> lessonTables;
    private ScheduleDto currentSchedule;
    private LessonBufferViewModel buffer;
    private LessonTableViewModel currentTable;

    public ScheduleViewModel(ScheduleServices scheduleServices, StudyGroupServices studyGroupServices, LessonNumberServices lessonNumberServices, LessonServices lessonServices)
    {
        this.scheduleServices = scheduleServices;
        this.studyGroupServices = studyGroupServices;
        this.lessonNumberServices = lessonNumberServices;
        this.lessonServices = lessonServices;
        
        LoadSchedules();
        currentSchedule = scheduleList.FirstOrDefault();
        currentTable = lessonTables.TryGetValue(currentSchedule, out var table) ? table : null;
    }
    
    public ICommand ChooseScheduleCommand { get; }
    public ICommand SaveScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }

    public AvaloniaList<ScheduleDto> ScheduleList
    {
        get => scheduleList;
        set => SetProperty(ref scheduleList, value);
    }

    public AvaloniaDictionary<ScheduleDto, LessonTableViewModel> LessonTables
    {
        get => lessonTables;
        set => SetProperty(ref lessonTables, value);
    }

    public ScheduleDto CurrentSchedule
    {
        get => currentSchedule;
        set => SetProperty(ref currentSchedule, value);
    }

    public LessonBufferViewModel Buffer
    {
        get => buffer;
        set => SetProperty(ref buffer, value);
    }

    public LessonTableViewModel Table
    {
        get => currentTable;
        set => SetProperty(ref currentTable, value);
    }

    private void LoadSchedules()
    {
        var schedules = scheduleServices.FetchSchedulesFromBackendAsync().Result;
        var tables = new Dictionary<ScheduleDto, LessonTableViewModel>();
        foreach (var schedule in schedules)
        {
            tables[schedule] = new LessonTableViewModel(
                schedule,
                lessonNumberServices,
                studyGroupServices,
                lessonServices);
        }
        ScheduleList = new AvaloniaList<ScheduleDto>(schedules);
        LessonTables = new AvaloniaDictionary<ScheduleDto, LessonTableViewModel>(tables);
    }
}