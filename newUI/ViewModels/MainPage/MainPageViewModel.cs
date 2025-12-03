using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleCreation;
using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.MainPage;

public class MainPageViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    public AvaloniaList<ScheduleDto> Schedules { get; } = new AvaloniaList<ScheduleDto>();

    public string SearchText { get; set; } = string.Empty;

    public ICommand AddScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }
    public ICommand ClearSchedulesCommand { get; }

    public MainPageViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddScheduleCommand = new RelayCommandAsync(AddSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedules);
        ClearSchedulesCommand = new RelayCommandAsync(ClearSchedules);
    }

    private async Task AddSchedule()
    {
        var scope = scopeFactory.CreateScope();
        var vm = scope.ServiceProvider.GetRequiredService<ScheduleCreationViewModel>();

        // Подписываемся на событие
        vm.ScheduleCreated += schedule =>
        {
            // Добавляем в коллекцию на UI-потоке
            Avalonia.Threading.Dispatcher.UIThread.Post(() => Schedules.Add(schedule));
        };

        windowManager.ShowWindow(vm); // метод ShowWindow должен устанавливать DataContext
    }

    private Task ClearSchedules()
    {
        Schedules.Clear();
        return Task.CompletedTask;
    }

    private Task LoadSchedules()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var fetchedItems = service.FetchSchedulesFromBackendAsync().Result;

        // Очищаем и добавляем по одному, чтобы уведомления сработали
        Schedules.Clear();
        foreach (var item in fetchedItems)
        {
            Schedules.Add(item);
        }

        return Task.CompletedTask;
    }
}
