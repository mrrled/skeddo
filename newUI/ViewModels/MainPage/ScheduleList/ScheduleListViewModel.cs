using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleEditor;
using newUI.ViewModels.SchedulePage.Schedule;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.MainPage.ScheduleList;

public class ScheduleListViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly NavigationService navigationService;
    private readonly IServiceProvider provider;

    private string searchText = string.Empty;

    public string SearchText
    {
        get => searchText;
        set
        {
            if (SetProperty(ref searchText, value))
                ApplyFilter();
        }
    }

    // Источник правды
    private readonly AvaloniaList<ScheduleItemViewModel> allItems = new();

    // Коллекция для UI
    public AvaloniaList<ScheduleItemViewModel> ScheduleItems { get; } = new();

    public ICommand AddScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }

    public ScheduleListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory,
        NavigationService navigationService, IServiceProvider provider)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        this.navigationService = navigationService;
        this.provider = provider;

        AddScheduleCommand = new RelayCommandAsync(AddSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedules);

        _ = LoadSchedules();
    }

    private async Task LoadSchedules()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var fetchedItems = await service.FetchSchedulesFromBackendAsync();

        allItems.Clear();
        ScheduleItems.Clear();

        foreach (var schedule in fetchedItems)
        {
            var itemVm = new ScheduleItemViewModel(schedule);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            ScheduleItems.Add(itemVm);
        }
    }

    private async Task SubscribeItemEvents(ScheduleItemViewModel itemVm)
    {
        itemVm.RequestSelect += item =>
        {
            var scheduleVm = provider.GetRequiredService<ScheduleViewModel>();

            if (scheduleVm != null && scheduleVm.ScheduleList != null)
            {
                var scheduleDto = scheduleVm.ScheduleList.FirstOrDefault(s => s.Id == item.Schedule.Id);
                if (scheduleDto != null)
                {
                    scheduleVm.CurrentSchedule = scheduleDto;
                }
            }

            navigationService.Navigate<ScheduleViewModel>();
        };

        itemVm.RequestDelete += async item =>
        {
            var confirmVm = new ConfirmDeleteViewModel(
                message: $"Вы уверены, что хотите удалить \"{item.Name}\"?"
            );

            var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

            if (result != true) return;
            
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            await service.DeleteSchedule(item.Schedule);

            // Удаляем из обеих коллекций
            allItems.Remove(item);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ScheduleItems.Remove(item));
        };

        itemVm.RequestEdit += async item =>
        {
            using var scope = scopeFactory.CreateScope();
            var vm = new ScheduleEditorViewModel(scopeFactory, item.Schedule);

            vm.ScheduleSaved += updatedSchedule =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    item.Name = updatedSchedule.Name;
                    ApplyFilter();
                });
            };

            await windowManager.ShowDialog<ScheduleEditorViewModel, ScheduleDto>(vm);
        };
    }

    private async Task AddSchedule()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new ScheduleEditorViewModel(scopeFactory);

        vm.ScheduleSaved += async schedule =>
        {
            var itemVm = new ScheduleItemViewModel(schedule);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            Avalonia.Threading.Dispatcher.UIThread.Post(ApplyFilter);
        };

        await windowManager.ShowDialog<ScheduleEditorViewModel, ScheduleDto>(vm);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? allItems
            : new AvaloniaList<ScheduleItemViewModel>(
                allItems.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );

        ScheduleItems.Clear();
        foreach (var item in filtered)
            ScheduleItems.Add(item);
    }
}