using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonTableViewModel : 
    DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    public event Action? TableUpdated;
    
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    
    
    private ScheduleDto schedule;
    private AvaloniaList<LessonNumberDto> lessonNumbers;
    private AvaloniaList<StudyGroupDto> studyGroups;
    private bool isInitialized;
    
    private bool isLoading;

    public LessonTableViewModel(ScheduleDto schedule,
        IServiceScopeFactory scopeFactory, 
        IWindowManager windowManager)
    {
        this.schedule = schedule;
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        _ = InitializeAsync();
    }
    
    private async Task InitializeAsync()
    {
        await LoadStudyGroupsAsync();
        await LoadLessonNumbersAsync();
        LoadDataToGrid();
        isInitialized = true;
    }
    
    public async Task RefreshAsync(ScheduleDto newSchedule = null)
    {
        try
        {
            IsLoading = true;
            
            if (newSchedule != null)
            {
                schedule = newSchedule;
            }
            
            if (isInitialized)
            {
                await LoadStudyGroupsAsync();
                await LoadLessonNumbersAsync();
                LoadDataToGrid();
                
                // Уведомляем об обновлении
                TableUpdated?.Invoke();
                OnPropertyChanged(nameof(Rows));
                OnPropertyChanged(nameof(Columns));
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public ScheduleDto Schedule
    {
        get => schedule;
        set => SetProperty(ref schedule, value);
    }
    
    public bool IsLoading
    {
        get => isLoading;
        private set => SetProperty(ref isLoading, value);
    }

    public AvaloniaList<LessonNumberDto> LessonNumbers
    {
        get => lessonNumbers;
        set => SetProperty(ref lessonNumbers, value);
    }

    public AvaloniaList<StudyGroupDto> StudyGroups
    {
        get => studyGroups;
        set => SetProperty(ref studyGroups, value);
    }

    private async Task LoadStudyGroupsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();
        var groups = await service.FetchStudyGroupsFromBackendAsync();
        StudyGroups = new AvaloniaList<StudyGroupDto>(groups);
    }

    private async Task LoadLessonNumbersAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();
        var numbers = await service.GetLessonNumbersByScheduleId(Schedule.Id);
        LessonNumbers = new AvaloniaList<LessonNumberDto>(numbers);
    }

    private void LoadDataToGrid()
    {
        var data = LoadData().Result;
        LoadDataFromBackend(data);
        
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
    }

    private async Task<
        List<
            (LessonNumberDto RowHeader,
            Dictionary<StudyGroupDto,
                LessonCardViewModel?> CellData)
        >> LoadData()
    {
        var result = new List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel?>)>();
        using var scope = scopeFactory.CreateScope();
        
        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var schedules = await scheduleService.FetchSchedulesFromBackendAsync();
        var currentSchedule = schedules.FirstOrDefault(s => s.Id == Schedule.Id);
    
        if (currentSchedule != null) 
            Schedule = currentSchedule;
        
        var lessonDictionary = new Dictionary<(int, string), LessonDto>();
        foreach (var lesson in Schedule.Lessons)
        {
            if (lesson.LessonNumber != null && lesson.StudyGroup != null) 
                lessonDictionary[(lesson.LessonNumber.Number, lesson.StudyGroup.Name)] = lesson;
        }

        foreach (var lessonNumber in LessonNumbers)
        {
            var rowData = new Dictionary<StudyGroupDto, LessonCardViewModel?>();
            
            foreach (var studyGroup in StudyGroups)
            {
                if (lessonDictionary.TryGetValue((lessonNumber.Number, studyGroup.Name), out var lesson))
                {
                    var card = new LessonCardViewModel(scopeFactory, windowManager)
                    {
                        Lesson = lesson
                    };
                    card.LessonClicked += OnLessonClicked;
                    rowData[studyGroup] = card;
                }
                else
                {
                    rowData[studyGroup] = new LessonCardViewModel(scopeFactory, windowManager, isVisible: false);
                }
            }
            
            result.Add((lessonNumber, rowData));
        }
        
        return result;
    }
    
    private void OnLessonClicked(LessonDto lesson)
    {
        // Можно вызвать событие для родительского ViewModel
        // или обработать здесь
        Console.WriteLine($"Lesson clicked: {lesson.Id}");
    }

    protected override LessonCardViewModel CreateEmptyCell()
    {
        throw new System.NotImplementedException();
    }
}