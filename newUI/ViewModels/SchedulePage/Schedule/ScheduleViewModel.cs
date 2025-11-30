using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class ScheduleViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    
    private AvaloniaList<ScheduleDto> scheduleList;
    private AvaloniaDictionary<ScheduleDto, LessonTableViewModel> lessonTables;
    private ScheduleDto currentSchedule;
    private LessonBufferViewModel buffer;
    private LessonTableViewModel currentTable;
    private bool isLoading;

    public ScheduleViewModel( IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;

        Buffer = new LessonBufferViewModel();

        ChooseScheduleCommand = new RelayCommandAsync(ChooseSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedulesAsync);
        SaveScheduleCommand = new RelayCommandAsync(SaveScheduleAsync);
        
        _ = InitializeAsync();
    }
    
    public ICommand ChooseScheduleCommand { get; }
    public ICommand SaveScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }
    
    public bool IsLoading
    {
        get => isLoading;
        set => SetProperty(ref isLoading, value);
    }

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
        set 
        { 
            if (SetProperty(ref currentSchedule, value))
            {
                if (value != null && LessonTables.ContainsKey(value))
                {
                    Table = LessonTables[value];
                }
            }
        }
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
    
    private async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            await LoadSchedulesAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private Task ChooseSchedule()
    {
        if (CurrentSchedule != null && LessonTables.ContainsKey(CurrentSchedule)) 
            Table = LessonTables[CurrentSchedule];
        
        return Task.CompletedTask;
    }

    private async Task SaveScheduleAsync()
    {
        // // Реализация сохранения расписания
    }

    private async Task LoadSchedulesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var schedules = await service.FetchSchedulesFromBackendAsync();
        var tables = new Dictionary<ScheduleDto, LessonTableViewModel>();
        
        foreach (var schedule in schedules)
        {
            tables[schedule] = new LessonTableViewModel(
                schedule,
                scopeFactory);
        }
        
        ScheduleList = new AvaloniaList<ScheduleDto>(schedules);
        LessonTables = new AvaloniaDictionary<ScheduleDto, LessonTableViewModel>(tables);
        
        if (ScheduleList.Any())
        {
            CurrentSchedule = ScheduleList.First();
            if (LessonTables.ContainsKey(CurrentSchedule))
            {
                Table = LessonTables[CurrentSchedule];
            }
        }
    }
}