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
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectEditor;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectList;

public class SchoolSubjectListViewModel : ViewModelBase
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
    private readonly AvaloniaList<SchoolSubjectItemViewModel> allItems = new();

    // Коллекция для UI
    public AvaloniaList<SchoolSubjectItemViewModel> SchoolSubjectItems { get; } = new();

    public ICommand AddSchoolSubjectCommand { get; }
    public ICommand LoadSchoolSubjectsCommand { get; }

    public SchoolSubjectListViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        AddSchoolSubjectCommand = new RelayCommandAsync(AddSchoolSubject);
        LoadSchoolSubjectsCommand = new RelayCommandAsync(LoadSchoolSubjects);

        _ = LoadSchoolSubjects();
    }

    private async Task LoadSchoolSubjects()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();
        var fetchedItems = await service.FetchSchoolSubjectsFromBackendAsync();

        allItems.Clear();
        SchoolSubjectItems.Clear();

        foreach (var schoolSubject in fetchedItems)
        {
            var itemVm = new SchoolSubjectItemViewModel(schoolSubject);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            SchoolSubjectItems.Add(itemVm);
        }
    }

    private async Task SubscribeItemEvents(SchoolSubjectItemViewModel itemVm)
    {
        itemVm.RequestDelete += async item =>
        {
            var confirmVm = new ConfirmDeleteViewModel(
                message: $"Вы уверены, что хотите удалить \"{item.Name}\"?"
            );

            var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

            if (result != true) return;
            
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();
            await service.DeleteSchoolSubject(item.SchoolSubject);

            // Удаляем из обеих коллекций
            allItems.Remove(item);
            Avalonia.Threading.Dispatcher.UIThread.Post(() => SchoolSubjectItems.Remove(item));
        };

        itemVm.RequestEdit += async item =>
        {
            using var scope = scopeFactory.CreateScope();
            var vm = new SchoolSubjectEditorViewModel(scopeFactory, item.SchoolSubject, windowManager);

            vm.SchoolSubjectSaved += updatedSchoolSubject =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    item.Name = updatedSchoolSubject.Name;
                    ApplyFilter();
                });
            };

            await windowManager.ShowDialog<SchoolSubjectEditorViewModel, SchoolSubjectDto>(vm);
        };
    }

    private async Task AddSchoolSubject()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new SchoolSubjectEditorViewModel(scopeFactory, windowManager);

        vm.SchoolSubjectSaved += async schoolSubject =>
        {
            var itemVm = new SchoolSubjectItemViewModel(schoolSubject);
            await SubscribeItemEvents(itemVm);
            allItems.Add(itemVm);
            Avalonia.Threading.Dispatcher.UIThread.Post(ApplyFilter);
        };

        await windowManager.ShowDialog<SchoolSubjectEditorViewModel, SchoolSubjectDto>(vm);
    }

    private void ApplyFilter()
    {
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? allItems
            : new AvaloniaList<SchoolSubjectItemViewModel>(
                allItems.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );

        UpdateSchoolSubjectItems(filtered);
    }

    private void UpdateSchoolSubjectItems(IEnumerable<SchoolSubjectItemViewModel> items)
    {
        SchoolSubjectItems.Clear();
        foreach (var item in items)
            SchoolSubjectItems.Add(item);
    }
}