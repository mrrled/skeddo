using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonTableViewModel : 
    DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    private readonly IServiceScopeFactory scopeFactory;
    
    private ScheduleDto schedule;
    private AvaloniaList<LessonNumberDto> lessonNumbers;
    private AvaloniaList<StudyGroupDto> studyGroups;

    public LessonTableViewModel(ScheduleDto schedule, IServiceScopeFactory scopeFactory)
    {
        this.schedule = schedule;
        this.scopeFactory = scopeFactory;
        InitializeAsync();
    }
    
    private void InitializeAsync()
    {
        LoadStudyGroupsAsync();
        LoadLessonNumbersAsync();
        LoadDataToGrid();
    }

    public ScheduleDto Schedule
    {
        get => schedule;
        set => SetProperty(ref schedule, value);
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

    private async void LoadStudyGroupsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();
        var groups = await service.FetchStudyGroupsFromBackendAsync();
        StudyGroups = new AvaloniaList<StudyGroupDto>(groups);
    }

    private async void LoadLessonNumbersAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();
        var numbers = await service.GetLessonNumbersByScheduleId(Schedule.Id);
        LessonNumbers = new AvaloniaList<LessonNumberDto>(numbers);
    }
    
    protected void LoadDataToGrid()
    {
        LoadDataFromBackend(() => LoadData().Result);
    }

    private async Task<List<(LessonNumberDto RowHeader, Dictionary<StudyGroupDto, LessonCardViewModel> CellData)>> LoadData()
    {
        var result = new List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>)>();
    
        // Загружаем актуальное расписание с уроками
        using var scope = scopeFactory.CreateScope();
        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var schedules = await scheduleService.FetchSchedulesFromBackendAsync();
        var currentSchedule = schedules.FirstOrDefault(s => s.Id == Schedule.Id);
    
        if (currentSchedule != null)
        {
            Schedule = currentSchedule; // Обновляем расписание
        }
        
        var lessonDictionary = new Dictionary<(int, string), LessonDto>();
        foreach (var lesson in Schedule.Lessons)
        {
            if (lesson.LessonNumber != null && lesson.StudyGroup != null)
            {
                lessonDictionary[(lesson.LessonNumber.Number, lesson.StudyGroup.Name)] = lesson;
            }
        }

        foreach (var lessonNumber in LessonNumbers)
        {
            var rowData = new Dictionary<StudyGroupDto, LessonCardViewModel>();
            
            foreach (var studyGroup in StudyGroups)
            {
                if (lessonDictionary.TryGetValue((lessonNumber.Number, studyGroup.Name), out var lesson))
                {
                    var viewModel = new LessonCardViewModel(scopeFactory) { Lesson = lesson };
                    rowData[studyGroup] = viewModel;
                }
                else
                {
                    var viewModel = new LessonCardViewModel(scopeFactory);
                    rowData[studyGroup] = viewModel;
                }
            }
            
            result.Add((lessonNumber, rowData));
        }
        
        return result;
    }
}