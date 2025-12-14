using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.MainPage.ScheduleList;
using newUI.ViewModels.SchedulePage.Lessons;
using newUI.ViewModels.SchedulePage.Toolbar;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class AnotherScheduleViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly IExportServices exportServices;
    
    private ObservableCollection<ScheduleTabViewModel> tabs = new();
    private ScheduleTabViewModel selectedTab;
    private readonly NavigationService navigationService;
    private readonly IScheduleServices scheduleServices;
    private ToolbarViewModel toolbar;

    public AnotherScheduleViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager,
        IExportServices exportServices,
        NavigationService navigationService,
        IScheduleServices scheduleServices)
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
            deleteCommand: new RelayCommandAsync(DeleteScheduleAsync),
            closeCommand: new RelayCommandAsync(CloseWindowAsync)
        );

        Toolbar.RequestPdfExport += async () =>
        {
            if (CurrentSchedule != null)
                await exportServices.GeneratePdfAsync(CurrentSchedule.Id);
        };

        Toolbar.RequestExcelExport += async () =>
        {
            if (CurrentSchedule != null)
                await exportServices.GenerateExcelAsync(CurrentSchedule.Id);
        };

    }
    
    public bool HasTabs => Tabs?.Count > 0;
    public bool HasSelectedTab => SelectedTab != null;
    public bool NoTabs => !HasTabs;
    
    public ICommand AddLessonCommand { get; }
    public ICommand LoadCurrentScheduleCommand { get; }
    public ICommand OpenScheduleListCommand { get; }
    
    private void OpenScheduleList()
    {
        navigationService.Navigate<ScheduleListViewModel>();
    }
    
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
                {
                    tab.IsSelected = tab == value;
                }
                OnPropertyChanged(nameof(Buffer));
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
    
    public async Task LoadSchedule(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var schedule = await service.GetScheduleByIdAsync(id);
        
        var existingTab = Tabs.FirstOrDefault(t => t.Id == id);
        
        if (existingTab != null)
        {
            existingTab.Update(schedule);
            SelectedTab = existingTab;
        }
        else
        {
            var tableViewModel = new LessonTableViewModel(schedule, scopeFactory);
            var tab = new ScheduleTabViewModel(
                schedule, 
                tableViewModel, 
                scopeFactory,
                CloseTabById,
                new LessonBufferViewModel(scopeFactory));
            
            Tabs.Add(tab);
            SelectedTab = tab;
        }
    }
    
    private void CloseTabById(int tabId)
    {
        var tab = Tabs.FirstOrDefault(t => t.Id == tabId);
        if (tab == null) return;
        
        Tabs.Remove(tab);
        if (SelectedTab == tab)
        {
            SelectedTab = Tabs.LastOrDefault();
        }
        OnPropertyChanged(nameof(HasTabs));
        OnPropertyChanged(nameof(HasSelectedTab));
        OnPropertyChanged(nameof(NoTabs));
    }
    
    public async Task AddLessonAsync()
    {
        var id = CurrentSchedule?.Id;
        if (id == null) return;
        
        var vm = new LessonCreationViewModel(scopeFactory, id.Value);
        vm.LessonCreated += async lesson =>
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            await service.AddLesson(lesson, id.Value);
            
            await LoadSchedule(id.Value);
        };
        
        vm.Window = windowManager.ShowWindow(vm);
    }
    
    private async Task SaveScheduleAsync()
    {
        // полный чилл, ниче не делаем
    }

    private async Task DeleteScheduleAsync()
    {
        if (CurrentSchedule == null)
            return;
        await scheduleServices.DeleteSchedule(CurrentSchedule);
        CloseTabById(CurrentSchedule.Id);
    }

    private async Task CloseWindowAsync()
    {
        if (CurrentSchedule == null)
            return;
        CloseTabById(CurrentSchedule.Id);
    }
}