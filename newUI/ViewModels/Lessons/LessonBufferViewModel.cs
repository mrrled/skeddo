using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;

namespace newUI.ViewModels.Lessons;

public class LessonBufferViewModel : ViewModelBase
{
    private AvaloniaList<LessonCardViewModel> lessonCards = new();
    
    public AvaloniaList<LessonCardViewModel> LessonCards
    {
        get => lessonCards;
        set => SetProperty(ref lessonCards, value);
    }
    
    public ICommand AddLessonCommand { get; }

    public LessonBufferViewModel()
    {
        
    }

    private Task AddLessonAsync(LessonCardViewModel lessonCard)
    {
        lessonCards.Add(lessonCard);
        OnPropertyChanged(nameof(LessonCards));
        return Task.CompletedTask;
    }
}