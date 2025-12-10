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
using newUI.ViewModels.Shared;
using newUI.ViewModels.TeachersPage.TeacherEditor;

namespace newUI.ViewModels.TeachersPage.TeacherList;

public class TeacherListViewModel : ViewModelBase
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
    private readonly AvaloniaList<TeacherItemViewModel> allItems = new();

    // Коллекция для UI
    public AvaloniaList<TeacherItemViewModel> TeacherItems { get; } = new();

    public ICommand AddTeacherCommand { get; }
    public ICommand LoadTeachersCommand { get; }

    public TeacherListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddTeacherCommand = new RelayCommandAsync(AddTeacher);
        LoadTeachersCommand = new RelayCommandAsync(LoadTeachers);

        _ = LoadTeachers();
    }

    private async Task LoadTeachers()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
        var fetchedItems = await service.FetchTeachersFromBackendAsync();

        allItems.Clear();
        TeacherItems.Clear();

        foreach (var teacher in fetchedItems)
        {
            var itemVm = new TeacherItemViewModel(teacher);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            TeacherItems.Add(itemVm);
        }
    }

    private async Task SubscribeItemEvents(TeacherItemViewModel itemVm)
    {
        itemVm.RequestDelete += async item =>
        {
            var confirmVm = new ConfirmDeleteViewModel(
                message: $"Вы уверены, что хотите удалить \"{item.FullName}\"?"
            );

            var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

            if (result != true) return;
            
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
            await service.DeleteTeacher(item.Teacher);

            // Удаляем из обеих коллекций
            allItems.Remove(item);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => TeacherItems.Remove(item));
        };

        itemVm.RequestEdit += async item =>
        {
            using var scope = scopeFactory.CreateScope();
            var vm = new TeacherEditorViewModel(scopeFactory, item.Teacher);

            vm.TeacherSaved += updatedTeacher =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    item.Surname = updatedTeacher.Surname;
                    item.Name = updatedTeacher.Name;
                    item.Patronymic = updatedTeacher.Patronymic;
                    ApplyFilter();
                });
            };

            await windowManager.ShowDialog<TeacherEditorViewModel, TeacherDto>(vm);
        };
    }

    private async Task AddTeacher()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new TeacherEditorViewModel(scopeFactory);

        vm.TeacherSaved += async teacher =>
        {
            var itemVm = new TeacherItemViewModel(teacher);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            Avalonia.Threading.Dispatcher.UIThread.Post(ApplyFilter);
        };

        await windowManager.ShowDialog<TeacherEditorViewModel, TeacherDto>(vm);
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            UpdateTeacherItems(allItems);
            return;
        }

        var tokens = SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .ToArray();

        var filtered = allItems.Where(teacher =>
            tokens.All(token =>
                teacher.Surname.Contains(token, StringComparison.OrdinalIgnoreCase)
                || teacher.Name.Contains(token, StringComparison.OrdinalIgnoreCase)
                || teacher.Patronymic.Contains(token, StringComparison.OrdinalIgnoreCase)
            )
        );

        UpdateTeacherItems(filtered);
    }

    private void UpdateTeacherItems(IEnumerable<TeacherItemViewModel> items)
    {
        TeacherItems.Clear();
        foreach (var item in items)
            TeacherItems.Add(item);
    }
}