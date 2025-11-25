using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Application.Services;

namespace newUI.ViewModels.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto lesson;
    public ILessonServices Service;
    
    public double Width { get; set; }
    public double Height { get; set; }

    public LessonCardViewModel(ILessonServices service)
    {
        Service = service;
    }

    public LessonDto Lesson
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