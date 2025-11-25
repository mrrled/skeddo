using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using newUI.ViewModels.Lessons;

namespace newUI.ViewModels.Schedule;

public class LessonBufferViewModel : ViewModelBase
{
    private AvaloniaList<LessonCardViewModel> lessonCards = new();
    
    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => lessonCards;
        set => SetProperty(ref lessonCards, value);
    }
    
    public ICommand AddLessonCommand { get; }

    private Task AddLessonAsync(LessonCardViewModel lessonCard)
    {
        lessonCards.Add(lessonCard);
        OnPropertyChanged(nameof(LessonCards));
        return Task.CompletedTask;
    }
}