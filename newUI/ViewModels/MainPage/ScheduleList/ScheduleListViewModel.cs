using System.Collections.ObjectModel;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Application.DtoModels;
using Application.IServices;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleEditor;
using newUI.ViewModels.MainPage.ScheduleList;
using System.Threading.Tasks;
using System.Windows.Input;

namespace newUI.ViewModels.MainPage;

public class ScheduleListViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    public AvaloniaList<ScheduleItemViewModel> ScheduleItems { get; set; } = new();

    public ICommand AddScheduleCommand { get; }
    public ICommand LoadSchedulesCommand { get; }

    public ScheduleListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddScheduleCommand = new RelayCommandAsync(AddSchedule);
        LoadSchedulesCommand = new RelayCommandAsync(LoadSchedules);

        _ = LoadSchedules();
    }

    private async Task LoadSchedules()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var fetchedItems = await service.FetchSchedulesFromBackendAsync();

        var items = new AvaloniaList<ScheduleItemViewModel>();
        foreach (var schedule in fetchedItems)
        {
            var itemVm = new ScheduleItemViewModel(schedule);
            SubscribeItemEvents(itemVm);
            items.Add(itemVm);
        }

        ScheduleItems = items;
    }

    private void SubscribeItemEvents(ScheduleItemViewModel itemVm)
    {
        itemVm.RequestDelete += async item =>
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            await service.DeleteSchedule(item.Schedule);

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
                    var index = ScheduleItems.IndexOf(item);
                    if (index >= 0)
                    {
                        var newItemVm = new ScheduleItemViewModel(updatedSchedule);
                        SubscribeItemEvents(newItemVm);
                        ScheduleItems[index] = newItemVm;
                    }
                });
            };

            await windowManager.ShowDialog<ScheduleEditorViewModel, ScheduleDto>(vm);
        };
    }

    private async Task AddSchedule()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new ScheduleEditorViewModel(scopeFactory);

        vm.ScheduleSaved += schedule =>
        {
            var itemVm = new ScheduleItemViewModel(schedule);
            SubscribeItemEvents(itemVm);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ScheduleItems.Add(itemVm));
        };

        await windowManager.ShowDialog<ScheduleEditorViewModel, ScheduleDto>(vm);
    }
}