using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonBufferViewModel : ViewModelBase
{
    private AvaloniaDictionary<Guid, LessonDraftDto> lessonDictionary = new();
    private AvaloniaDictionary<int, LessonCardViewModel> lessonCardViewModels = new();
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;

    public LessonBufferViewModel(
        IServiceScopeFactory scopeFactory,
        IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        ClearCommand = new RelayCommandAsync(ClearAsync);
    }

    public AvaloniaDictionary<Guid, LessonDraftDto> Lessons
    {
        get => lessonDictionary;
        set => SetProperty(ref lessonDictionary, value);
    }

    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => new (lessonCardViewModels.Select(x => x.Value));
    }
    
    public ICommand ClearCommand { get; }

    private Task ClearAsync()
    {
        Clear();
        return Task.CompletedTask;
    }

    public void Clear()
    {
        Lessons.Clear();
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
    }

    public void AddMany(IEnumerable<LessonDraftDto> lessonDrafts)
    {
        foreach (var lesson in lessonDrafts)
        {
            Lessons[lesson.Id] = lesson;
            var card = new LessonCardViewModel(
                scopeFactory, windowManager, () => OnPropertyChanged())
            {
                Lesson = lesson.ToLessonDto()
            };
            card.LessonClicked += OnLessonClicked;
            lessonCardViewModels[lesson.Id] = card;
        }
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
    }
    
    private void OnLessonClicked(LessonDto lesson)
    {
        Console.WriteLine($"Lesson clicked: {lesson.Id}");
    }
}