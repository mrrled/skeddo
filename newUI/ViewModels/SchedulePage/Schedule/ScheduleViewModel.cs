using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using newUI.Messages;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleList;
using newUI.ViewModels.SchedulePage.Lessons;
using newUI.ViewModels.SchedulePage.Toolbar;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class ScheduleViewModel : ViewModelBase, IRecipient<ScheduleDeletedMessage>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly IExportServices exportServices;
    private readonly NavigationService navigationService;
    private readonly IScheduleServices scheduleServices;

    private ObservableCollection<ScheduleTabViewModel> tabs = new();
    private ScheduleTabViewModel selectedTab;
    private ToolbarViewModel toolbar;

    public ScheduleViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager,
        IExportServices exportServices,
        NavigationService navigationService,
        IScheduleServices scheduleServices,
        IFileService fileService)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.exportServices = exportServices;
        this.navigationService = navigationService;
        this.scheduleServices = scheduleServices;

        AddLessonCommand = new AsyncRelayCommand(AddLessonAsync);
        LoadCurrentScheduleCommand = new AsyncRelayCommand(LoadCurrentScheduleAsync);
        OpenScheduleListCommand = new RelayCommand(OpenScheduleList);

        Toolbar = new ToolbarViewModel(
            saveCommand: new RelayCommandAsync(SaveScheduleAsync),
            deleteCommand: new RelayCommandAsync(OnDeleteClickedAsync),
            closeCommand: new RelayCommandAsync(CloseWindowAsync)
        );

        Toolbar.RequestPdfExport += async () =>
        {
            if (CurrentSchedule == null) return;

            var file = await fileService.SaveFileAsync(
                "Сохранить PDF-расписание",
                ".pdf",
                $"Расписание_{CurrentSchedule.Name}.pdf"
            );
            if (file == null) return;

            try
            {
                await using var stream = await file.OpenWriteAsync();
                await exportServices.GeneratePdfAsync(CurrentSchedule.Id, stream);
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel("Экспорт в PDF успешно завершен!"));
            }
            catch (Exception ex)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel($"Ошибка при сохранении: {ex.Message}"));
            }
        };

        Toolbar.RequestExcelExport += async () =>
        {
            if (CurrentSchedule == null) return;

            var file = await fileService.SaveFileAsync(
                "Сохранить Excel-расписание",
                ".xlsx",
                $"Расписание_{CurrentSchedule.Name}.xlsx"
            );
            if (file == null) return;

            try
            {
                await using var stream = await file.OpenWriteAsync();
                await exportServices.GenerateExcelAsync(CurrentSchedule.Id, stream);
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel("Экспорт в Excel успешно завершен!"));
            }
            catch (Exception ex)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel($"Ошибка при сохранении: {ex.Message}"));
            }
        };

        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(ScheduleDeletedMessage message)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() => CloseTabById(message.ScheduleId));
    }

    public bool HasTabs => Tabs?.Count > 0;
    public bool HasSelectedTab => SelectedTab != null;
    public bool NoTabs => !HasTabs;

    public ICommand AddLessonCommand { get; }
    public ICommand LoadCurrentScheduleCommand { get; }
    public ICommand OpenScheduleListCommand { get; }

    private void OpenScheduleList() => navigationService.Navigate<ScheduleListViewModel>();

    public ObservableCollection<ScheduleTabViewModel> Tabs
    {
        get => tabs;
        set => SetProperty(ref tabs, value);
    }

    public ToolbarViewModel Toolbar
    {
        get => toolbar;
        set => SetProperty(ref toolbar, value);
    }

    public ScheduleTabViewModel SelectedTab
    {
        get => selectedTab;
        set
        {
            if (SetProperty(ref selectedTab, value) && value != null)
            {
                foreach (var tab in Tabs)
                    tab.IsSelected = tab == value;

                OnPropertyChanged(nameof(Buffer));
                OnPropertyChanged(nameof(CurrentSchedule));
                OnPropertyChanged(nameof(CurrentScheduleTable));
            }

            Toolbar.IsEnabled = selectedTab != null;
        }
    }

    public ScheduleDto CurrentSchedule => SelectedTab?.Schedule;
    public LessonTableViewModel CurrentScheduleTable => SelectedTab?.TableViewModel;
    public LessonBufferViewModel Buffer => SelectedTab?.LessonBuffer;

    private async Task LoadCurrentScheduleAsync()
    {
        if (SelectedTab?.Schedule?.Id != null)
            await LoadSchedule(SelectedTab.Schedule.Id);
    }

    public async Task LoadSchedule(Guid id)
    {
        var existingTab = Tabs.FirstOrDefault(t => t.Id == id);

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var scheduleResult = await service.GetScheduleByIdAsync(id);
        if (scheduleResult.IsFailure)
        {
            await windowManager.ShowDialog<NotificationViewModel, object?>(
                new NotificationViewModel(scheduleResult.Error));
            return;
        }

        var schedule = scheduleResult.Value;

        if (existingTab != null)
        {
            existingTab.Update(schedule);
            SelectedTab = existingTab;
        }
        else
        {
            var tableViewModel = new LessonTableViewModel(schedule, scopeFactory, windowManager);
            var bufferViewModel = new LessonBufferViewModel(scopeFactory, windowManager, id);

            tableViewModel.RequestEditLesson += async lesson => await EditLessonAsync(lesson);
            bufferViewModel.RequestEditLesson += async lesson => await EditLessonAsync(lesson);
            bufferViewModel.RequestTableRefresh += async () => await RefreshActiveTabContentAsync();
            tableViewModel.LessonMovedToBuffer += async draft => await RefreshActiveTabContentAsync();

            var tab = new ScheduleTabViewModel(schedule, tableViewModel, scopeFactory, CloseTabById, bufferViewModel);
            Tabs.Add(tab);
            SelectedTab = tab;
        }
    }

    private async Task RefreshActiveTabContentAsync()
    {
        if (Buffer != null)
            await Buffer.RefreshAsync();

        if (CurrentScheduleTable != null)
            await CurrentScheduleTable.RefreshAsync();
    }

    private void CloseTabById(Guid tabId)
    {
        var tab = Tabs.FirstOrDefault(t => t.Id == tabId);
        if (tab == null) return;

        Tabs.Remove(tab);
        if (SelectedTab == tab)
            SelectedTab = Tabs.LastOrDefault();

        OnPropertyChanged(nameof(HasTabs));
        OnPropertyChanged(nameof(HasSelectedTab));
        OnPropertyChanged(nameof(NoTabs));
    }

    public async Task AddLessonAsync()
    {
        var id = CurrentSchedule?.Id;
        if (id == null) return;

        var vm = new LessonEditorViewModel(scopeFactory, windowManager, id.Value);
        vm.LessonSaved += async result => await RefreshActiveTabContentAsync();
        await windowManager.ShowDialog<LessonEditorViewModel, object?>(vm);
    }

    public async Task EditLessonAsync(LessonDto lesson)
    {
        var id = CurrentSchedule?.Id;
        if (id == null) return;

        var vm = new LessonEditorViewModel(scopeFactory, windowManager, lesson);
        vm.LessonSaved += async result => await RefreshActiveTabContentAsync();
        vm.LessonDeleted += async _ => await RefreshActiveTabContentAsync();
        await windowManager.ShowDialog<LessonEditorViewModel, object?>(vm);
    }

    private async Task SaveScheduleAsync() => await Task.CompletedTask;

    private async Task OnDeleteClickedAsync()
    {
        if (CurrentSchedule == null) return;

        var confirmVm =
            new ConfirmDeleteViewModel($"Вы уверены, что хотите удалить расписание \"{CurrentSchedule.Name}\"?");
        var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

        if (result == true)
        {
            var id = CurrentSchedule.Id;
            await scheduleServices.DeleteSchedule(CurrentSchedule);
            CloseTabById(id);
            WeakReferenceMessenger.Default.Send(new ScheduleDeletedMessage(id));
        }
    }

    private async Task CloseWindowAsync()
    {
        if (CurrentSchedule != null) CloseTabById(CurrentSchedule.Id);
        await Task.CompletedTask;
    }
}