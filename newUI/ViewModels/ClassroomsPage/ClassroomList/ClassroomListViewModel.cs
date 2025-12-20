using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.ClassroomsPage.ClassroomEditor;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.ClassroomsPage.ClassroomList;

public class ClassroomListViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

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
    private readonly AvaloniaList<ClassroomItemViewModel> allItems = new();

    // Коллекция для UI
    public AvaloniaList<ClassroomItemViewModel> ClassroomItems { get; } = new();

    public ICommand AddClassroomCommand { get; }
    public ICommand LoadClassroomsCommand { get; }

    public ClassroomListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddClassroomCommand = new RelayCommandAsync(AddClassroom);
        LoadClassroomsCommand = new RelayCommandAsync(LoadClassrooms);

        _ = LoadClassrooms();
    }

    private async Task LoadClassrooms()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClassroomServices>();
        var fetchedItems = await service.FetchClassroomsFromBackendAsync();

        allItems.Clear();
        ClassroomItems.Clear();

        foreach (var classroom in fetchedItems)
        {
            var itemVm = new ClassroomItemViewModel(classroom);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            ClassroomItems.Add(itemVm);
        }
    }

    private async Task SubscribeItemEvents(ClassroomItemViewModel itemVm)
    {
        itemVm.RequestDelete += async item =>
        {
            var confirmVm = new ConfirmDeleteViewModel(
                message: $"Вы уверены, что хотите удалить \"{item.Name}\"?"
            );

            var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

            if (result != true) return;
            
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IClassroomServices>();
            await service.DeleteClassroom(item.Classroom);

            // Удаляем из обеих коллекций
            allItems.Remove(item);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ClassroomItems.Remove(item));
        };

        itemVm.RequestEdit += async item =>
        {
            using var scope = scopeFactory.CreateScope();
            var vm = new ClassroomEditorViewModel(scopeFactory, item.Classroom, windowManager);

            vm.ClassroomSaved += updatedClassroom =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    item.Name = updatedClassroom.Name;
                    ApplyFilter();
                });
            };

            await windowManager.ShowDialog<ClassroomEditorViewModel, ClassroomDto>(vm);
        };
    }

    private async Task AddClassroom()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new ClassroomEditorViewModel(scopeFactory, windowManager);

        vm.ClassroomSaved += async classroom =>
        {
            var itemVm = new ClassroomItemViewModel(classroom);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            Avalonia.Threading.Dispatcher.UIThread.Post(ApplyFilter);
        };

        await windowManager.ShowDialog<ClassroomEditorViewModel, ClassroomDto>(vm);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? allItems
            : new AvaloniaList<ClassroomItemViewModel>(
                allItems.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );

        UpdateClassroomItems(filtered);
    }

    private void UpdateClassroomItems(IEnumerable<ClassroomItemViewModel> items)
    {
        ClassroomItems.Clear();
        foreach (var item in items)
            ClassroomItems.Add(item);
    }
}