using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.DtoExtensions;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using Domain.Models;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto? lesson;
    private bool isVisible;
    private int columnSpan = 1;
    private bool isGroupWideLesson;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly Action? refreshCallback;

    public event Action<LessonDto>? LessonUpdated;
    public event Action<LessonDraftDto>? LessonDowngraded;
    public event Action<Guid>? LessonDeleted;

    public LessonCardViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager,
        Action? refreshCallback = null, bool isVisible = true)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.refreshCallback = refreshCallback;
        this.isVisible = isVisible;
        ClickCommand = new AsyncRelayCommand(OnClick);
    }

    public bool IsVisible
    {
        get => isVisible;
        set => SetProperty(ref isVisible, value);
    }

    public int ColumnSpan
    {
        get => columnSpan;
        set => SetProperty(ref columnSpan, value);
    }

    public bool IsGroupWideLesson
    {
        get => isGroupWideLesson;
        set => SetProperty(ref isGroupWideLesson, value);
    }

    public LessonDto? Lesson
    {
        get => lesson;
        set
        {
            if (SetProperty(ref lesson, value))
            {
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    public string Color => lesson?.WarningType switch
    {
        WarningType.Conflict => "LightCoral",
        WarningType.Warning => "LemonChiffon",
        _ => "Transparent"
    };
    
    public event Action<LessonDto>? EditRequested;
    

    public ICommand ClickCommand { get; }

    private async Task OnClick()
    {
        if (Lesson == null) return;

        LessonEditorViewModel editVm;

        if (Lesson.Id == Guid.Empty)
        {
            editVm = new LessonEditorViewModel(scopeFactory, windowManager, Lesson.ScheduleId)
            {
                SelectedStudyGroup = Lesson.StudyGroup,
                SelectedLessonNumber = Lesson.LessonNumber,
                SelectedStudySubgroup = Lesson.StudySubgroup
            };
        }
        else
        {
            editVm = new LessonEditorViewModel(scopeFactory, windowManager, Lesson);
        }

        editVm.LessonSaved += result =>
        {
            if (result == null) return;

            if (result.IsDraft && result.LessonDraft != null)
            {
                Lesson = result.LessonDraft.ToLessonDto();
                LessonDowngraded?.Invoke(result.LessonDraft);
            }
            else if (result.Lesson != null)
            {
                Lesson = result.Lesson;
                LessonUpdated?.Invoke(result.Lesson);
            }

            // Это вызывает Buffer.RefreshAsync()
            refreshCallback?.Invoke();
        };

        editVm.LessonDeleted += id =>
        {
            LessonDeleted?.Invoke(id);
            refreshCallback?.Invoke();
        };

        EditRequested?.Invoke(Lesson);
    }
}