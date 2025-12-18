using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    : DynamicGridViewModel<LessonCardViewModel, ColumnViewModel, LessonNumberDto>
{
    public event Action? TableUpdated;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    private bool isInitialized;
    private bool isLoading;

    private ScheduleDto Schedule;

    public AvaloniaList<StudyGroupDto> StudyGroups { get; private set; } = new();
    public AvaloniaList<LessonNumberDto> LessonNumbers { get; private set; } = new();

    public ObservableCollection<ColumnViewModel> FlatColumns { get; } = new();
    public ObservableCollection<StudyGroupDto> GroupHeaders { get; } = new();

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

        BuildColumns();
    }

    private void BuildColumns()
    {
        FlatColumns.Clear();
        GroupHeaders.Clear();

        foreach (var group in StudyGroups)
        {
            GroupHeaders.Add(group);

            if (group.StudySubgroups?.Count > 0)
            {
                foreach (var subgroup in group.StudySubgroups)
                {
                    FlatColumns.Add(new ColumnViewModel
                    {
                        StudyGroup = group,
                        StudySubgroup = subgroup,
                        DisplayName = subgroup.Name ?? group.Name
                    });
                }
            }
            else
            {
                FlatColumns.Add(new ColumnViewModel
                {
                    StudyGroup = group,
                    StudySubgroup = null,
                    DisplayName = group.Name
                });
            }
        }

        Columns = new ObservableCollection<ColumnViewModel>(FlatColumns);
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
        Rows.Clear();
        OnPropertyChanged(nameof(Rows));
        var data = LoadData().Result;
        LoadDataFromBackend(data);

        OnPropertyChanged(nameof(Rows));
        OnPropertyChanged(nameof(Columns));
        DebugRows();
    }

    private async Task<List<(LessonNumberDto RowHeader, Dictionary<ColumnViewModel, LessonCardViewModel?> CellData)>> LoadData()
{
    var result = new List<(LessonNumberDto, Dictionary<ColumnViewModel, LessonCardViewModel?>)>();
    using var scope = scopeFactory.CreateScope();

    var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
    var currentSchedule = await scheduleService.GetScheduleByIdAsync(Schedule.Id);
    
    if (currentSchedule != null) 
        Schedule = currentSchedule;
    
    var lessonDictionary = new Dictionary<(int, Guid, string?), LessonDto>();
    
    foreach (var lesson in Schedule.Lessons)
    {
        if (lesson.LessonNumber != null && lesson.StudyGroup != null) 
        {
            var subgroupName = lesson.StudySubgroup?.Name;
            lessonDictionary[(lesson.LessonNumber.Number, lesson.StudyGroup.Id, subgroupName)] = lesson;
        }
    }

    foreach (var lessonNumber in LessonNumbers)
    {
        var rowData = new Dictionary<ColumnViewModel, LessonCardViewModel?>();
        var columnIndex = 0;
        
        while (columnIndex < FlatColumns.Count)
        {
            var currentColumn = FlatColumns[columnIndex];
            var currentGroupId = currentColumn.StudyGroup.Id;
            
            var specificLessonKey = (lessonNumber.Number, currentGroupId, currentColumn.StudySubgroup?.Name);
            var groupWideLessonKey = (lessonNumber.Number, currentGroupId, (string?)null);
            
            bool hasSpecificLesson = lessonDictionary.TryGetValue(specificLessonKey, out var specificLesson);
            bool hasGroupWideLesson = lessonDictionary.TryGetValue(groupWideLessonKey, out var groupWideLesson);
            
            LessonDto lessonToUse = null;
            var span = 1;
            bool isGroupWide = false;
            
            if (hasSpecificLesson)
            {
                lessonToUse = specificLesson;
            }
            else if (hasGroupWideLesson)
            {
                lessonToUse = groupWideLesson;
                isGroupWide = true;
                
                span = 1;
                while (columnIndex + span < FlatColumns.Count)
                {
                    var nextColumn = FlatColumns[columnIndex + span];
                    if (nextColumn.StudyGroup.Id != currentGroupId)
                        break;
                    
                    var nextSpecificKey = (lessonNumber.Number, nextColumn.StudyGroup.Id, nextColumn.StudySubgroup?.Name);
                    if (lessonDictionary.ContainsKey(nextSpecificKey))
                        break;
                        
                    span++;
                }
            }
            
            // Создаем ячейку
            LessonCardViewModel card;
            
            if (lessonToUse != null)
            {
                card = new LessonCardViewModel(scopeFactory, windowManager, Refresh)
                {
                    Lesson = lessonToUse,
                    ColumnSpan = span,
                    IsGroupWideLesson = isGroupWide
                };
            }
            else
            {
                card = new LessonCardViewModel(scopeFactory, windowManager, Refresh, isVisible: false)
                {
                    Lesson = new LessonDto
                    {
                        StudyGroup = currentColumn.StudyGroup,
                        StudySubgroup = currentColumn.StudySubgroup,
                        LessonNumber = lessonNumber
                    },
                    ColumnSpan = 1,
                    IsGroupWideLesson = false
                };
            }
            
            card.LessonClicked += OnLessonClicked;
            rowData[currentColumn] = card;
            
            columnIndex += span;
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

        vm.StudyGroupSaved += async _ => { await RefreshAsync(); };

        vm.StudyGroupDeleted += async _ => { await RefreshAsync(); };

        await windowManager.ShowDialog<StudyGroupEditorViewModel, StudyGroupDto>(vm);
    }

    private async void OpenEditStudyGroupEditor(StudyGroupDto studyGroup)
    {
        var vm = new StudyGroupEditorViewModel(windowManager, scopeFactory, studyGroup);

        vm.StudyGroupSaved += async _ => { await RefreshAsync(); };

        vm.StudyGroupDeleted += async _ => { await RefreshAsync(); };

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

        vm.LessonNumberSaved += async _ => { await RefreshAsync(); };

        vm.LessonNumberDeleted += async _ => { await RefreshAsync(); };

        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    private async void OpenEditLessonNumberEditor(LessonNumberDto lessonNumber)
    {
        var vm = new LessonNumberEditorViewModel(
            windowManager,
            scopeFactory,
            lessonNumber,
            Schedule.Id);

        vm.LessonNumberSaved += async _ => { await RefreshAsync(); };

        vm.LessonNumberDeleted += async _ => { await RefreshAsync(); };

        await windowManager.ShowDialog<LessonNumberEditorViewModel, LessonNumberDto>(vm);
    }

    protected override LessonCardViewModel CreateEmptyCell()
    {
        return new LessonCardViewModel(
            scopeFactory,
            windowManager,
            Refresh,
            isVisible: false)
        {
            Lesson = new LessonDto()
        };
    }

    private int GetNextLessonNumber()
    {
        if (!LessonNumbers.Any())
            return 1;

        return LessonNumbers.Max(x => x.Number) + 1;
    }
    
    public void DebugRows()
    {
        Console.WriteLine($"=== ДЕБАГ ROWS ===");
        Console.WriteLine($"Rows.Count: {Rows.Count}");
    
        foreach (var row in Rows)
        {
            Console.WriteLine($"Row {row.RowHeader.Number}, Cells: {row.Cells.Count}");
        
            foreach (var cell in row.Cells)
            {
                var card = cell as LessonCardViewModel;
                if (card != null)
                {
                    Console.WriteLine($"  - Visible: {card.IsVisible}, " +
                                      $"Span: {card.ColumnSpan}, " +
                                      $"GroupWide: {card.IsGroupWideLesson}");
                }
            }
        }
    }

    //костыль - не трогать
    public override List<TableDataRow<LessonCardViewModel, ColumnViewModel, LessonNumberDto>> 
        CreateRows(List<(LessonNumberDto RowHeader, Dictionary<ColumnViewModel, LessonCardViewModel?> CellData)> data,
            List<ColumnViewModel> newColumns)
    {
        var newRows = new List<TableDataRow<LessonCardViewModel, ColumnViewModel, LessonNumberDto>>();
        foreach (var (lessonNumber, cellsDict) in data)
        {
            var columns = new List<ColumnViewModel>();
            var cells = new List<LessonCardViewModel>();
            foreach (var column in FlatColumns)
            {
                if (cellsDict.TryGetValue(column, out var cellData) && cellData != null)
                {
                    columns.Add(column);
                    cells.Add(cellData);
                }
            }
            var row = new TableDataRow<LessonCardViewModel, ColumnViewModel, LessonNumberDto>(lessonNumber, columns);
            for (var i = 0; i < cells.Count; i++)
            {
                row.SetCell(columns[i], cells[i]);
            }
            
            newRows.Add(row);
        }
        return newRows;
    }
}