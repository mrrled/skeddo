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

namespace newUI.ViewModels.SchedulePage.Schedule;

public class AnotherScheduleViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    
    private LessonBufferViewModel buffer;
    private ObservableCollection<ScheduleTabViewModel> tabs = new();
    private ScheduleTabViewModel selectedTab;
    private readonly NavigationService navigationService;

    public AnotherScheduleViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager,
        NavigationService navigationService)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.navigationService = navigationService;
        
        buffer = new LessonBufferViewModel(scopeFactory);
        
        AddLessonCommand = new AsyncRelayCommand(AddLessonAsync);
        LoadCurrentScheduleCommand = new AsyncRelayCommand(LoadCurrentScheduleAsync);
        OpenScheduleListCommand = new RelayCommand(OpenScheduleList);
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
            }
        }
    }
    
    public ScheduleDto CurrentSchedule => SelectedTab?.Schedule;
    public LessonTableViewModel CurrentScheduleTable => SelectedTab?.TableViewModel;
    
    public LessonBufferViewModel Buffer
    {
        get => buffer;
        set => SetProperty(ref buffer, value);
    }
    
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
        buffer.AddMany(schedule.LessonDrafts);
        
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
                CloseTabById);
            
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
    }
    
    public void OpenSchedule(ScheduleDto schedule)
    {
        var existingTab = Tabs.FirstOrDefault(t => t.Id == schedule.Id);
        
        if (existingTab != null)
        {
            SelectedTab = existingTab;
        }
        else
        {
            var tableViewModel = new LessonTableViewModel(schedule, scopeFactory);
            var tab = new ScheduleTabViewModel(
                schedule, 
                tableViewModel, 
                scopeFactory, 
                CloseTabById);
            
            Tabs.Add(tab);
            SelectedTab = tab;
        }
    }
    
    public Task AddLessonAsync()
    {
        var id = CurrentSchedule?.Id;
        if (id == null) return Task.CompletedTask;
        
        var vm = new LessonCreationViewModel(scopeFactory, id.Value);
        vm.LessonCreated += async lesson =>
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            await service.AddLesson(lesson, id.Value);
            
            await LoadSchedule(id.Value);
        };
        
        vm.Window = windowManager.ShowWindow(vm);
        return Task.CompletedTask;
    }
}