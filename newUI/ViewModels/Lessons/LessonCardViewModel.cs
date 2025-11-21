using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.Services;

namespace newUI.ViewModels.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private DtoLesson lesson;
    public IService Service;
    
    public double Width { get; set; }
    public double Height { get; set; }

    public LessonCardViewModel(IService service)
    {
        Service = service;
    }

    public DtoLesson Lesson
    {
        get => lesson;
        set => SetProperty(ref lesson, value);
    }

    public void SetOnClickCommand(Func<Task> command)
    {
        OnClickCommand = new RelayCommandAsync(command);
    }
    
    public ICommand? OnClickCommand { get; private set; } 
    //у нас скорее всего будет несколько различных сущностей, использующих LessonCard
    //Команду OnClick будем задавать из них самостоятельно
    
    //Либо нахуй и просто наследника этого класса бахнем
}