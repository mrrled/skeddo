using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.DtoExtensions; // Для ToLessonDto()
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using Domain.Models;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto lesson;
    private bool isVisible;
    private int columnSpan = 1;
    private bool isGroupWideLesson;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly Action? refreshCallback;

    // События
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

    public LessonDto Lesson
    {
        get => lesson;
        set
        {
            if (SetProperty(ref lesson, value))
            {
                // При обновлении урока обязательно пересчитываем цвет
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    // Логика цвета на основе предупреждений
    public string Color => lesson?.WarningType switch
    {
        WarningType.Conflict => "LightCoral", // Ошибка/Конфликт
        WarningType.Warning => "LemonChiffon", // Предупреждение
        _ => "White" // Всё ок
    };

    public ICommand ClickCommand { get; }

    private async Task OnClick()
    {
        if (Lesson == null) return;

        // Создаем редактор: если Id пустой — режим создания, иначе — редактирования
        LessonEditorViewModel editVm = Lesson.Id == Guid.Empty
            ? new LessonEditorViewModel(scopeFactory, windowManager, Lesson.ScheduleId)
            {
                SelectedStudyGroup = Lesson.StudyGroup,
                SelectedLessonNumber = Lesson.LessonNumber
            }
            : new LessonEditorViewModel(scopeFactory, windowManager, Lesson);

        // Подписка на сохранение
        editVm.LessonSaved += result =>
        {
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

            refreshCallback?.Invoke();
        };

        // Подписка на удаление
        editVm.LessonDeleted += id =>
        {
            LessonDeleted?.Invoke(id);
            refreshCallback?.Invoke();
        };

        await windowManager.ShowDialog<LessonEditorViewModel, object?>(editVm);
    }
}