using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Avalonia.Collections;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonBufferViewModel : ViewModelBase
{
    private AvaloniaList<LessonCardViewModel> lessonCards = new();

    public LessonBufferViewModel()
    {
        AddLessonCommand = new RelayCommandAsync(AddLessonAsync);
    }

    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => lessonCards;
        set => SetProperty(ref lessonCards, value);
    }
    
    public ICommand AddLessonCommand { get; }

    private Task AddLessonAsync()
    {
        return Task.CompletedTask;
    }

    public void AddLessonToBuffer(LessonCardViewModel lesson)
    {
        LessonCards.Add(lesson);
    }
}