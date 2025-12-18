using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.DtoExtensions; // Для ToLessonDto()
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using Domain.Models; // Проверь, чтобы WarningType был доступен здесь

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
                // При обновлении урока обязательно пересчитываем цвет для UI
                OnPropertyChanged(nameof(Color));
            }
        }
    }

    // Логика валидации через цвета
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

        // Создаем ViewModel редактора в зависимости от того, пустая это ячейка или существующий урок
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
                // Если бизнес-логика "понизила" урок до черновика
                Lesson = result.LessonDraft.ToLessonDto();
                LessonDowngraded?.Invoke(result.LessonDraft);
            }
            else if (result.Lesson != null)
            {
                // Если всё успешно
                LessonUpdated?.Invoke(result.Lesson);
            }

            refreshCallback?.Invoke();
        };

        editVm.LessonDeleted += _ => refreshCallback?.Invoke();

        await windowManager.ShowDialog<LessonEditorViewModel, object?>(editVm);
    }
}