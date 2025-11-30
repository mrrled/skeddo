using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using newUI.ViewModels.SchedulePage.Schedule;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class ScheduleViewModel : ViewModelBase
{
    private readonly IScheduleServices scheduleServices;
    private readonly IStudyGroupServices studyGroupServices;
    private readonly ILessonNumberServices lessonNumberServices;
    private readonly ILessonServices lessonServices;
    
    private AvaloniaList<ScheduleDto> scheduleList;
    private AvaloniaDictionary<ScheduleDto, LessonTableViewModel> lessonTables;
    private ScheduleDto currentSchedule;
    private LessonBufferViewModel buffer;
    private LessonTableViewModel currentTable;

    public ScheduleViewModel(IScheduleServices scheduleServices,
        IStudyGroupServices studyGroupServices,
        ILessonNumberServices lessonNumberServices,
        ILessonServices lessonServices)
    {
        this.scheduleServices = scheduleServices;
        this.studyGroupServices = studyGroupServices;
        this.lessonNumberServices = lessonNumberServices;
        this.lessonServices = lessonServices;
        
        LoadSchedules();
        currentSchedule = scheduleList.FirstOrDefault();
        currentTable = lessonTables.TryGetValue(currentSchedule, out var table) ? table : null;

        ChooseScheduleCommand = new RelayCommandAsync(ChooseSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedulesAsync);
        SaveScheduleCommand = new RelayCommandAsync(SaveScheduleAsync);
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
    
    private Task ChooseSchedule()
    {
        if (CurrentSchedule != null && LessonTables.ContainsKey(CurrentSchedule))
        {
            Table = LessonTables[CurrentSchedule];
        }

        return Task.CompletedTask;
    }

    private async Task SaveScheduleAsync()
    {
        // // Реализация сохранения расписания
        // if (CurrentSchedule != null)
        // {
        //     await scheduleServices.SaveScheduleAsync(CurrentSchedule);
        // }
    }

    private async Task LoadSchedulesAsync()
    {
        var schedules = await scheduleServices.FetchSchedulesFromBackendAsync();
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
        
        if (ScheduleList.Any())
        {
            CurrentSchedule = ScheduleList.First();
            ChooseSchedule();
        }
    }
}