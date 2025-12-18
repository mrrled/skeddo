using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto lesson;
    private bool isVisible;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly Action? refreshCallback;

    public event Action<LessonDto>? LessonUpdated;
    public event Action<LessonDraftDto>? LessonDowngraded;

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

    public LessonDto Lesson
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
        Domain.Models.WarningType.Conflict => "LightCoral",
        Domain.Models.WarningType.Warning => "LemonChiffon",
        _ => "White"
    };

    public ICommand ClickCommand { get; }

    private async Task OnClick()
    {
        if (Lesson == null) return;

        // Определяем режим: создание на пустой ячейке или редактирование существующего
        LessonEditorViewModel editVm = Lesson.Id == Guid.Empty
            ? new LessonEditorViewModel(scopeFactory, windowManager, Lesson.ScheduleId)
            {
                SelectedStudyGroup = Lesson.StudyGroup,
                SelectedLessonNumber = Lesson.LessonNumber
            }
            : new LessonEditorViewModel(scopeFactory, windowManager, Lesson);

        editVm.LessonSaved += result =>
        {
            if (result.IsDraft && result.LessonDraft != null)
            {
                // Если урок стал черновиком (Downgraded)
                Lesson = result.LessonDraft.ToLessonDto();
                LessonDowngraded?.Invoke(result.LessonDraft);
            }
            else if (result.Lesson != null)
            {
                // Если урок успешно сохранен/обновлен
                LessonUpdated?.Invoke(result.Lesson);
            }

            refreshCallback?.Invoke();
        };

        editVm.LessonDeleted += _ => refreshCallback?.Invoke();

        await windowManager.ShowDialog<LessonEditorViewModel, object?>(editVm);
    }
}