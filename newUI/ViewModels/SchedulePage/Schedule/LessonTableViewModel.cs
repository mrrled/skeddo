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

public class LessonTableViewModel : DynamicGridViewModel<LessonCardViewModel, StudyGroupDto, LessonNumberDto>
{
    public event Action? TableUpdated;
    public event Action<LessonDraftDto>? LessonMovedToBuffer;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    private bool isInitialized;
    private bool isLoading;
    private ScheduleDto Schedule;

    public AvaloniaList<StudyGroupDto> StudyGroups { get; private set; } = new();
    public AvaloniaList<LessonNumberDto> LessonNumbers { get; private set; } = new();

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
        await LoadDataToGrid();
        isInitialized = true;
    }

    public async Task RefreshAsync(ScheduleDto? newSchedule = null)
    {
        try
        {
            IsLoading = true;
            if (newSchedule != null) Schedule = newSchedule;
            if (!isInitialized) return;

            await LoadStudyGroupsAsync();
            await LoadLessonNumbersAsync();
            await LoadDataToGrid();

            TableUpdated?.Invoke();
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

    private async Task LoadDataToGrid()
    {
        var data = await LoadData();
        LoadDataFromBackend(data);
        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
    }

    private async Task<List<(LessonNumberDto RowHeader, Dictionary<StudyGroupDto, LessonCardViewModel?> CellData)>>
        LoadData()
    {
        var result = new List<(LessonNumberDto, Dictionary<StudyGroupDto, LessonCardViewModel?>)>();
        using var scope = scopeFactory.CreateScope();

        var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var currentSchedule = await scheduleService.GetScheduleByIdAsync(Schedule.Id);
        if (currentSchedule != null) Schedule = currentSchedule;

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
                if (lessonDictionary.TryGetValue((lessonNumber.Number, studyGroup.Name), out var lesson))
                {
                    card = new LessonCardViewModel(scopeFactory, windowManager, Refresh) { Lesson = lesson };
                    card.LessonDowngraded += draft =>
                    {
                        LessonMovedToBuffer?.Invoke(draft);
                        _ = RefreshAsync();
                    };
                }
                else
                {
                    card = new LessonCardViewModel(scopeFactory, windowManager, Refresh)
                    {
                        Lesson = new LessonDto
                            { StudyGroup = studyGroup, LessonNumber = lessonNumber, ScheduleId = Schedule.Id }
                    };
                }

                rowData[studyGroup] = card;
            }

            result.Add((lessonNumber, rowData));
        }

        return result;
    }

    private void Refresh() => _ = RefreshAsync();

    private async void OpenAddStudyGroupEditor()
    {
        var vm = new StudyGroupEditorViewModel(windowManager, scopeFactory);
        vm.StudyGroupSaved += async _ => await RefreshAsync();
        vm.StudyGroupDeleted += async _ => await RefreshAsync();
        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    private async void OpenEditStudyGroupEditor(StudyGroupDto studyGroup)
    {
        var vm = new StudyGroupEditorViewModel(windowManager, scopeFactory, studyGroup);
        vm.StudyGroupSaved += async _ => await RefreshAsync();
        vm.StudyGroupDeleted += async _ => await RefreshAsync();
        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    private async void OpenAddLessonNumberEditor()
    {
        var vm = new LessonNumberEditorViewModel(windowManager, scopeFactory, GetNextLessonNumber(), Schedule.Id);
        vm.LessonNumberSaved += async _ => await RefreshAsync();
        vm.LessonNumberDeleted += async _ => await RefreshAsync();
        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    private async void OpenEditLessonNumberEditor(LessonNumberDto lessonNumber)
    {
        var vm = new LessonNumberEditorViewModel(windowManager, scopeFactory, lessonNumber, Schedule.Id);
        vm.LessonNumberSaved += async _ => await RefreshAsync();
        vm.LessonNumberDeleted += async _ => await RefreshAsync();
        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    protected override LessonCardViewModel CreateEmptyCell()
    {
        return new LessonCardViewModel(scopeFactory, windowManager, Refresh, isVisible: true)
        {
            Lesson = new LessonDto { Id = Guid.Empty }
        };
    }

    private int GetNextLessonNumber() => !LessonNumbers.Any() ? 1 : LessonNumbers.Max(x => x.Number) + 1;
}