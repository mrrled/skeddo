using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Application.DtoModels;
using Application.IServices;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleCreation;
using newUI.ViewModels.MainPage.ScheduleList;

namespace newUI.ViewModels.MainPage;

public class ScheduleListViewModel : ViewModelBase
{
    private AvaloniaList<ScheduleDto> schedules = new();
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    public AvaloniaList<ScheduleItemViewModel> ScheduleItems { get; set; } = new();

    public AvaloniaList<ScheduleDto> Schedules
    {
        get => schedules;
        set => SetProperty(ref schedules, value);
    }

    public string SearchText { get; set; } = string.Empty;

    public ICommand AddScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }

    public ScheduleListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddScheduleCommand = new RelayCommandAsync(AddSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedules);

        // Загружаем расписания при создании VM
        _ = LoadSchedules();
    }

    private async Task AddSchedule()
    {
        var scope = scopeFactory.CreateScope();
        var vm = scope.ServiceProvider.GetRequiredService<ScheduleCreationViewModel>();

        vm.ScheduleCreated += schedule =>
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
                ScheduleItems.Add(new ScheduleItemViewModel(schedule, windowManager)));
        };

        vm.Window = windowManager.ShowWindow(vm);
    }

    public async Task LoadSchedules()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var fetchedItems = await service.FetchSchedulesFromBackendAsync();

        var items = new AvaloniaList<ScheduleItemViewModel>();
        foreach (var schedule in fetchedItems)
        {
            items.Add(new ScheduleItemViewModel(schedule, windowManager));
        }

        ScheduleItems = items;
    }
}
