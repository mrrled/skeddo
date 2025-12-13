using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class AnotherScheduleViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    
    private AvaloniaDictionary<int, ScheduleDto> openedSchedules = new();
    private AvaloniaDictionary<int, LessonTableViewModel> openedScheduleTables = new();
    private LessonBufferViewModel buffer;
    
    private AvaloniaList<ScheduleSelectionItem> scheduleList = new();
    private ScheduleSelectionItem selectedSchedule;
    
    private ScheduleDto currentSchedule;
    private LessonTableViewModel currentScheduleTable;


    public AnotherScheduleViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        buffer = new LessonBufferViewModel(scopeFactory);
        AddLessonCommand = new AsyncRelayCommand(AddLessonAsync);
        LoadCurrentScheduleCommand = new AsyncRelayCommand(LoadCurrentScheduleAsync);
    }

    public ICommand AddLessonCommand { get; }
    public ICommand LoadCurrentScheduleCommand { get; }

    public AvaloniaList<ScheduleSelectionItem> ScheduleList
    {
        get => scheduleList;
        set => SetProperty(ref scheduleList, value);
        // get => new (openedSchedules.Select(x => new ScheduleSelectionItem(x.Key, x.Value.Name)));
    }

    public ScheduleSelectionItem SelectedSchedule
    {
        get => selectedSchedule;
        set
        {
            if (SetProperty(ref selectedSchedule, value))
            {
                if (!openedSchedules.TryGetValue(value.Id, out var schedule))
                {
                    Task.Run(async () => await LoadSchedule(value.Id));
                    return;
                }
                
                CurrentSchedule = schedule;
                
                if (!openedScheduleTables.TryGetValue(value.Id, out var table))
                {
                    table = new LessonTableViewModel(schedule, scopeFactory);
                    openedScheduleTables[value.Id] = table;
                }
            
                CurrentScheduleTable = table;
            }
        }
    }

    public LessonTableViewModel CurrentScheduleTable
    {
        get => currentScheduleTable;
        set => SetProperty(ref currentScheduleTable, value);
    }

    public ScheduleDto CurrentSchedule
    {
        get => currentSchedule;
        set => SetProperty(ref currentSchedule, value);
    }

    public AvaloniaDictionary<int, ScheduleDto> OpenedSchedules
    {
        get => openedSchedules;
        set => SetProperty(ref openedSchedules, value);
    }

    public AvaloniaDictionary<int, LessonTableViewModel> OpenedScheduleTables
    {
        get => openedScheduleTables;
        set => SetProperty(ref openedScheduleTables, value);
    }
    
    public LessonBufferViewModel Buffer
    {
        get => buffer;
        set => SetProperty(ref buffer, value);
    }

    private void AddSchedule(ScheduleDto schedule)
    {
        var id = schedule.Id;
        OpenedSchedules[id] = schedule;
        
        // Обновляем список для ComboBox
        if (ScheduleList.All(x => x.Id != id))
        {
            ScheduleList.Add(new ScheduleSelectionItem(id, schedule.Name));
        }
        else
        {
            // Обновляем имя если уже существует
            var item = ScheduleList.First(x => x.Id == id);
            item.Name = schedule.Name;
        }
        OnPropertyChanged(nameof(OpenedSchedules));
        OnPropertyChanged(nameof(OpenedScheduleTables));
        OnPropertyChanged(nameof(ScheduleList));
    }
    
    private async Task LoadCurrentScheduleAsync()
    {
        if (currentSchedule?.Id != null)
            await LoadSchedule(currentSchedule.Id);
    }

    public async Task LoadSchedule(int id)
    {
        // IsLoading = true;
        try
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            var schedule = await service.GetScheduleByIdAsync(id);
            buffer.AddMany(schedule.LessonDrafts);
        
            if (openedSchedules.ContainsKey(id))
            {
                openedSchedules[id] = schedule;
            
                if (openedScheduleTables.TryGetValue(id, out var table))
                {
                    await table.RefreshAsync(schedule);
                }
                else
                {
                    openedScheduleTables[id] = new LessonTableViewModel(schedule, scopeFactory);
                }
                
                OnPropertyChanged(nameof(OpenedSchedules));
                OnPropertyChanged(nameof(OpenedScheduleTables));
            
                if (selectedSchedule.Id == id)
                {
                    SelectedSchedule = new ScheduleSelectionItem(id, schedule.Name);
                }
            }
            else
            {
                AddSchedule(schedule);
                SelectedSchedule = new ScheduleSelectionItem(id, schedule.Name);
            }
        }
        finally
        {
            // IsLoading = false;
        }
    }
    
    public Task AddLessonAsync()
    {
        var id = CurrentSchedule.Id;
        var vm = new LessonCreationViewModel(scopeFactory, id);
        vm.LessonCreated += lesson =>
        {
            var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            service.AddLesson(lesson, id);
            Task.Run(async () => await LoadSchedule(id));
        };
        vm.Window = windowManager.ShowWindow(vm);
        return Task.CompletedTask;
    }
}