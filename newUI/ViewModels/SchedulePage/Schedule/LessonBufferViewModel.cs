using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Avalonia.Collections;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonBufferViewModel : ViewModelBase
{
    private AvaloniaDictionary<int, LessonDraftDto> lessonDictionary = new();
    private readonly IServiceScopeFactory scopeFactory;

    public LessonBufferViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        ClearCommand = new RelayCommandAsync(ClearAsync);
    }

    public AvaloniaDictionary<int, LessonDraftDto> Lessons
    {
        get => lessonDictionary;
        set => SetProperty(ref lessonDictionary, value);
    }

    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => new (lessonDictionary.Values
            .Select(lesson => new LessonCardViewModel(scopeFactory)
            {
                Lesson = lesson.ToLessonDto()
            }));
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

    public void AddLesson(LessonDraftDto lessonDrafts)
    {
        Lessons[lessonDrafts.Id] = lessonDrafts;
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
    }

    public void AddMany(IEnumerable<LessonDraftDto> lessonDrafts)
    {
        foreach (var lesson in lessonDrafts)
        {
            Lessons[lesson.Id] = lesson;
        }
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
    }
}