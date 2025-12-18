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
using newUI.ViewModels.SchedulePage.LessonNumbers;

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
    public IRelayCommand<LessonNumberDto> EditLessonNumberCommand { get; }

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

        AddLessonNumberCommand = new RelayCommand(OpenAddLessonNumberEditor);
        EditLessonNumberCommand = new RelayCommand<LessonNumberDto>(OpenEditLessonNumberEditor);

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
    // StudyGroup Editor
    // -------------------------

    private async void OpenAddStudyGroupEditor()
    {
        var vm = new StudyGroupEditorViewModel(windowManager, scopeFactory);

        vm.StudyGroupSaved += async _ =>
        {
            await RefreshAsync();
        };

        vm.StudyGroupDeleted += async _ =>
        {
            await RefreshAsync();
        };

        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    private async void OpenEditStudyGroupEditor(StudyGroupDto studyGroup)
    {
        var vm = new StudyGroupEditorViewModel(windowManager, scopeFactory, studyGroup);

        vm.StudyGroupSaved += async _ =>
        {
            await RefreshAsync();
        };

        vm.StudyGroupDeleted += async _ =>
        {
            await RefreshAsync();
        };

        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    // -------------------------
    // LessonNumber Editor
    // -------------------------

    private async void OpenAddLessonNumberEditor()
    {
        var nextNumber = GetNextLessonNumber();

        var vm = new LessonNumberEditorViewModel(
            windowManager,
            scopeFactory,
            nextNumber,
            Schedule.Id);

        vm.LessonNumberSaved += async _ =>
        {
            await RefreshAsync();
        };

        vm.LessonNumberDeleted += async _ =>
        {
            await RefreshAsync();
        };

        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    private async void OpenEditLessonNumberEditor(LessonNumberDto lessonNumber)
    {
        var vm = new LessonNumberEditorViewModel(
            windowManager,
            scopeFactory,
            lessonNumber,
            Schedule.Id);

        vm.LessonNumberSaved += async _ =>
        {
            await RefreshAsync();
        };

        vm.LessonNumberDeleted += async _ =>
        {
            await RefreshAsync();
        };

        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    protected override LessonCardViewModel CreateEmptyCell()
    {
        return new LessonCardViewModel(
            scopeFactory,
            windowManager,
            Refresh,
            isVisible: false);
    }
    
    private int GetNextLessonNumber()
    {
        if (!LessonNumbers.Any())
            return 1;

        return LessonNumbers.Max(x => x.Number) + 1;
    }
}