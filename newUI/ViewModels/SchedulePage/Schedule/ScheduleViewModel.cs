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
        
        LoadCurrentScheduleCommand = new RelayCommandAsync(LoadCurrentSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedulesAsync);
        SaveScheduleCommand = new RelayCommandAsync(SaveScheduleAsync);
        AddScheduleCommand = new RelayCommandAsync(AddScheduleAsync);
        
        _ = InitializeAsync();
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
    
    public ICommand SaveScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }
    public ICommand LoadCurrentScheduleCommand { get; }
    public ICommand AddScheduleCommand { get; }
    
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
                if (value != null && LessonTables != null && LessonTables.TryGetValue(value, out var table))
                {
                    Table = table;
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

    private async Task AddScheduleAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetService<IScheduleServices>();
        var newSchedule = new ScheduleDto();
        await service.AddSchedule(newSchedule);
        await LoadSchedulesAsync();
    }

    private Task SaveScheduleAsync()
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<IScheduleServices>();
            service.EditSchedule(currentSchedule, currentSchedule); 
            //TODO: сделать копию расписания, чтобы был oldSchedule
        }
        return Task.CompletedTask;
    }

    private async Task LoadCurrentSchedule()
    {
        IsLoading = true;
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            var schedule = await service.GetScheduleByIdAsync(currentSchedule.Id);
            lessonTables.Remove(CurrentSchedule);
            scheduleList.Remove(CurrentSchedule);
            scheduleList.Add(schedule);
            lessonTables.Add(schedule, new LessonTableViewModel(schedule, scopeFactory, buffer));
            CurrentSchedule = schedule;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadSchedulesAsync()
    {
        IsLoading = true;
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            var schedules = await service.FetchSchedulesFromBackendAsync();
            var tables = new Dictionary<ScheduleDto, LessonTableViewModel>();
            
            foreach (var schedule in schedules)
            {
                tables[schedule] = new LessonTableViewModel(schedule, scopeFactory, buffer);
            }
            
            ScheduleList = new AvaloniaList<ScheduleDto>(schedules);
            LessonTables = new AvaloniaDictionary<ScheduleDto, LessonTableViewModel>(tables);
            
            if (ScheduleList.Count > 0 && CurrentSchedule == null)
            {
                CurrentSchedule = ScheduleList.First();
            }
            else if (CurrentSchedule != null)
            {
                var updatedSchedule = ScheduleList.FirstOrDefault(s => s.Id == CurrentSchedule.Id);
                CurrentSchedule = updatedSchedule ?? ScheduleList.FirstOrDefault();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}