using System;
using System.Windows.Input;
using Application.DtoModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class ScheduleTabViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly Action<int> onCloseTab;
    private LessonBufferViewModel lessonBuffer;
    private string title;
    private bool isSelected;
    
    public ScheduleTabViewModel(
        ScheduleDto schedule,
        LessonTableViewModel tableViewModel,
        IServiceScopeFactory scopeFactory,
        Action<int> onCloseTab,
        LessonBufferViewModel lessonBuffer)
    {
        Schedule = schedule;
        TableViewModel = tableViewModel;
        this.scopeFactory = scopeFactory;
        this.onCloseTab = onCloseTab;
        this.lessonBuffer = lessonBuffer;
        LessonBuffer.AddMany(schedule.LessonDrafts);

        Title = schedule.Name;
        CloseCommand = new RelayCommand(() => onCloseTab?.Invoke(Id));
    }
    
    public int Id => Schedule.Id;

    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    public LessonBufferViewModel LessonBuffer
    {
        get => lessonBuffer;
        set => SetProperty(ref lessonBuffer, value);
    }
    public ScheduleDto Schedule { get; private set; }
    public LessonTableViewModel TableViewModel { get; }
    public ICommand CloseCommand { get; }
    
    public void Update(ScheduleDto newSchedule)
    {
        Schedule = newSchedule;
        Title = newSchedule.Name;
        TableViewModel.RefreshAsync(newSchedule).Wait();
        
        LessonBuffer.Clear();
        LessonBuffer.AddMany(newSchedule.LessonDrafts);
        
        OnPropertyChanged(nameof(LessonBuffer));
        OnPropertyChanged(nameof(Title));
    }
}