using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DtoModels;
using Application.Services;
using Avalonia.Collections;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonTableViewModel : 
    DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    private StudyGroupServices studyGroupServices;
    private LessonNumberServices lessonNumberServices;
    private LessonServices lessonServices;
    
    private ScheduleDto schedule;
    private AvaloniaList<LessonNumberDto> lessonNumbers;
    private AvaloniaList<StudyGroupDto> studyGroups;

    public LessonTableViewModel(ScheduleDto schedule, LessonNumberServices lessonNumberServices, StudyGroupServices studyGroupServices, LessonServices lessonServices)
    {
        this.schedule = schedule;
        this.lessonNumberServices = lessonNumberServices;
        this.studyGroupServices = studyGroupServices;
        this.lessonServices = lessonServices;
        LoadStudyGroups();
        LoadLessonNumbers();
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

    private void LoadStudyGroups()
    {
        var groups = studyGroupServices
            .FetchStudyGroupsFromBackendAsync()
            .Result;
        StudyGroups = new AvaloniaList<StudyGroupDto>(groups);
    }

    private void LoadLessonNumbers()
    {
        var numbers = lessonNumberServices
            .GetLessonNumbersByScheduleId(Schedule.Id)
            .Result;
        LessonNumbers = new AvaloniaList<LessonNumberDto>(numbers);
    }

    private Task<List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>)>> LoadData()
    {
        var result = new List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>)>();
        var rows = new Dictionary<LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>>();
        foreach (var number in LessonNumbers)
        {
            rows[number] = new Dictionary<StudyGroupDto, LessonCardViewModel>();
        }

        foreach (var lesson in Schedule.Lessons)
        {
            var viewModel = new LessonCardViewModel(lessonServices){ Lesson = lesson };
            rows[lesson.LessonNumber!][lesson.StudyGroup!] = viewModel;
        }
        
        return Task.FromResult(result);
    }
}