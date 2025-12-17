using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Helpers;
using newUI.ViewModels.SchedulePage.Lessons;
using Application.DtoModels;
using Application.IServices;
using newUI.ViewModels.SchedulePage.StudyGroups;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonTableViewModel
    : DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    public event Action? TableUpdated;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    private bool isInitialized;
    private bool isLoading;

    private ScheduleDto Schedule;

    public AvaloniaList<StudyGroupDto> StudyGroups { get; private set; } = new();
    public AvaloniaList<LessonNumberDto> LessonNumbers { get; private set; } = new();

    // Команды
    public IRelayCommand AddStudyGroupCommand { get; }
    public IRelayCommand AddLessonNumberCommand { get; }
    public IRelayCommand<StudyGroupDto> EditStudyGroupCommand { get; }

    public LessonTableViewModel(
        ScheduleDto schedule,
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager)
    {
        Schedule = schedule;
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;

        AddStudyGroupCommand = new RelayCommand(OpenAddStudyGroupEditor);
        EditStudyGroupCommand = new RelayCommand<StudyGroupDto>(OpenEditStudyGroupEditor);
        AddLessonNumberCommand = new RelayCommand(AddRow);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadStudyGroupsAsync();
        await LoadLessonNumbersAsync();
        LoadDataToGrid();
        isInitialized = true;
    }

    public async Task RefreshAsync(ScheduleDto? newSchedule = null)
    {
        try
        {
            IsLoading = true;

            if (newSchedule != null)
                Schedule = newSchedule;

            if (!isInitialized)
                return;

            await LoadStudyGroupsAsync();
            await LoadLessonNumbersAsync();
            LoadDataToGrid();

            TableUpdated?.Invoke();
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(Columns));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public bool IsLoading
    {
        get => isLoading;
        private set => SetProperty(ref isLoading, value);
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

    private async Task<List<(LessonNumberDto RowHeader,
        Dictionary<StudyGroupDto, LessonCardViewModel?> CellData)>> LoadData()
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
                LessonCardViewModel card;

                if (lessonDictionary.TryGetValue(
                        (lessonNumber.Number, studyGroup.Name),
                        out var lesson))
                {
                    card = new LessonCardViewModel(scopeFactory, windowManager, Refresh)
                    {
                        Lesson = lesson
                    };
                }
                else
                {
                    card = new LessonCardViewModel(
                        scopeFactory,
                        windowManager,
                        Refresh,
                        isVisible: false)
                    {
                        Lesson = new LessonDto
                        {
                            StudyGroup = studyGroup,
                            LessonNumber = lessonNumber
                        }
                    };
                }

                card.LessonClicked += OnLessonClicked;
                rowData[studyGroup] = card;
            }

            result.Add((lessonNumber, rowData));
        }

        return result;
    }

    private void Refresh()
    {
        RefreshAsync().Wait();
    }

    private void OnLessonClicked(LessonDto lesson)
    {
        Console.WriteLine($"Lesson clicked: {lesson.Id}");
    }

    // -------------------------
    // StudyGroup Editor logic
    // -------------------------

    private async void OpenAddStudyGroupEditor()
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new StudyGroupEditorViewModel(scopeFactory);

        vm.StudyGroupSaved += async studyGroup =>
        {
            StudyGroups.Add(studyGroup);
            await RefreshAsync();
        };

        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    private async void OpenEditStudyGroupEditor(StudyGroupDto studyGroup)
    {
        using var scope = scopeFactory.CreateScope();
        var vm = new StudyGroupEditorViewModel(scopeFactory, studyGroup);

        vm.StudyGroupSaved += async updatedGroup =>
        {
            var existing = StudyGroups.FirstOrDefault(x => x.Id == updatedGroup.Id);
            if (existing != null)
                existing.Name = updatedGroup.Name;

            await RefreshAsync();
        };

        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    // -------------------------
    // LessonNumber (+) — без изменений
    // -------------------------

    private void AddRow()
    {
        var newLessonNumber = new LessonNumberDto
        {
            Number = LessonNumbers.Count + 1
        };

        LessonNumbers.Add(newLessonNumber);
        LoadDataToGrid();
        TableUpdated?.Invoke();
    }

    protected override LessonCardViewModel CreateEmptyCell()
    {
        return new LessonCardViewModel(
            scopeFactory,
            windowManager,
            Refresh,
            isVisible: false);
    }
}