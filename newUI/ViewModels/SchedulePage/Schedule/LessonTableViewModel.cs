using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonTableViewModel : 
    DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    private readonly IStudyGroupServices studyGroupServices;
    private readonly ILessonNumberServices lessonNumberServices;
    private readonly ILessonServices lessonServices;
    
    private ScheduleDto schedule;
    private AvaloniaList<LessonNumberDto> lessonNumbers;
    private AvaloniaList<StudyGroupDto> studyGroups;

    public LessonTableViewModel(ScheduleDto schedule, 
        ILessonNumberServices lessonNumberServices,
        IStudyGroupServices studyGroupServices, 
        ILessonServices lessonServices)
    {
        this.schedule = schedule;
        this.lessonNumberServices = lessonNumberServices;
        this.studyGroupServices = studyGroupServices;
        this.lessonServices = lessonServices;
        LoadStudyGroups();
        LoadLessonNumbers();
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
    
    protected void LoadDataToGrid()
    {
        LoadDataFromBackend(() => LoadData().Result);
    }

    private Task<List<(LessonNumberDto RowHeader, Dictionary<StudyGroupDto, LessonCardViewModel> CellData)>> LoadData()
    {
        // var rows = new Dictionary<LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>>();
        // foreach (var number in LessonNumbers)
        // {
        //     rows[number] = new Dictionary<StudyGroupDto, LessonCardViewModel>();
        // }
        //
        // foreach (var lesson in Schedule.Lessons)
        // {
        //     var viewModel = new LessonCardViewModel(lessonServices){ Lesson = lesson };
        //     rows[lesson.LessonNumber][lesson.StudyGroup] = viewModel;
        // }
        //
        // var result = rows.Select(kvp => (RowHeader: kvp.Key, CellData: kvp.Value)).ToList();
        
        var result = new List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel>)>();
        var lessonDictionary = new Dictionary<(LessonNumberDto, StudyGroupDto), LessonDto>();
        foreach (var lesson in Schedule.Lessons)
        {
            if (lesson.LessonNumber != null && lesson.StudyGroup != null)
            {
                lessonDictionary[(lesson.LessonNumber, lesson.StudyGroup)] = lesson;
            }
        }

        foreach (var lessonNumber in LessonNumbers)
        {
            var rowData = new Dictionary<StudyGroupDto, LessonCardViewModel>();
            
            foreach (var studyGroup in StudyGroups)
            {
                if (lessonDictionary.TryGetValue((lessonNumber, studyGroup), out var lesson))
                {
                    var viewModel = new LessonCardViewModel(lessonServices) { Lesson = lesson };
                    rowData[studyGroup] = viewModel;
                }
                else
                {
                    var viewModel = new LessonCardViewModel(lessonServices) { Lesson = new LessonDto() };
                    rowData[studyGroup] = viewModel;
                }
            }
            
            result.Add((lessonNumber, rowData));
        }
        
        return Task.FromResult(result);
    }
}