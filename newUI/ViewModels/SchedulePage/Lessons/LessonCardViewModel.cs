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
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    public bool IsVisible { get; set; }
    public event Action<LessonDto>? LessonClicked;

    private bool isDragging;
    private bool isDragOver;

    private readonly Action? refreshCallback;

    public LessonCardViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager, Action? refreshCallback = null, bool isVisible = true)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.refreshCallback = refreshCallback;
        IsVisible = isVisible;

        ClickCommand = new AsyncRelayCommand(OnClick);
        StartDragCommand = new RelayCommand(StartDrag);
        DropCommand = new RelayCommand<LessonCardViewModel>(OnDrop);
    }

    public LessonDto Lesson
    {
        get => lesson;
        set => SetProperty(ref lesson, value);
    }

    public ICommand StartDragCommand { get; }
    public ICommand DropCommand { get; }
    public ICommand ClickCommand { get; }

    public bool IsDragging
    {
        get => isDragging;
        set => SetProperty(ref isDragging, value);
    }

    public bool IsDragOver
    {
        get => isDragOver;
        set => SetProperty(ref isDragOver, value);
    }

    private Task OnClick()
    {
        if (Lesson != null)
        {
            LessonClicked?.Invoke(Lesson);
            EditLesson();
        }

        return Task.CompletedTask;
    }

    private void EditLesson()
    {
        var editVm = new LessonEditorViewModel(scopeFactory, Lesson);
        editVm.LessonUpdated += updatedLesson =>
        {
            Lesson = updatedLesson;
            OnPropertyChanged(nameof(Lesson));
            refreshCallback.Invoke();
            // Можно вызвать событие для обновления таблицы
            // TableUpdated?.Invoke();
        };

        windowManager.ShowDialog<LessonEditorViewModel, LessonDto?>(editVm);
    }

    private void StartDrag()
    {
        IsDragging = true;
        Console.WriteLine($"Starting drag for lesson: {Lesson?.Id}");
    }

    private void OnDrop(LessonCardViewModel target)
    {
        if (target != null && target != this)
        {
            SwapLessons(target);
        }
    }

    private void SwapLessons(LessonCardViewModel target)
    {
        (Lesson.LessonNumber, target.Lesson.LessonNumber) = (target.Lesson.LessonNumber, Lesson.LessonNumber);
        (Lesson.StudyGroup, target.Lesson.StudyGroup) = (target.Lesson.StudyGroup, Lesson.StudyGroup);

        OnPropertyChanged(nameof(Lesson));
        target.OnPropertyChanged(nameof(Lesson));

        SaveSwappedLessons(target);
    }

    private async void SaveSwappedLessons(LessonCardViewModel target)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonServices>();

        if (Lesson != null && target.Lesson != null)
        {
            // Сохраняем оба урока с новыми позициями
            await service.EditLesson(Lesson, Lesson.ScheduleId);
            await service.EditLesson(target.Lesson, Lesson.ScheduleId);
        }
    }
}