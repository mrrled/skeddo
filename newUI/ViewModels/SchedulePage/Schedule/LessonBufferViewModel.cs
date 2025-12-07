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
    private AvaloniaList<LessonDto> lessons = new();
    private readonly IServiceScopeFactory scopeFactory;

    public LessonBufferViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        ClearCommand = new RelayCommandAsync(ClearAsync);
    }

    public AvaloniaList<LessonDto> Lessons
    {
        get => lessons;
        set => SetProperty(ref lessons, value);
    }

    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => new (lessons
            .Select(lesson => new LessonCardViewModel(scopeFactory)
            {
                Lesson = lesson
            }));
    }
    
    public ICommand ClearCommand { get; }

    private Task ClearAsync()
    {
        Lessons.Clear();
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
        return Task.CompletedTask;
    }

    public void AddLessonToBuffer(LessonDto lesson)
    {
        Lessons.Add(lesson);
        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(LessonCards));
    }
}