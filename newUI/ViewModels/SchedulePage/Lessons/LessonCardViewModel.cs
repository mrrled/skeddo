using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto lesson;
    private readonly IServiceScopeFactory scopeFactory;
    public bool IsVisible { get; set; }

    public LessonCardViewModel(IServiceScopeFactory scopeFactory, bool isVisible = true)
    {
        this.scopeFactory = scopeFactory;
        IsVisible = isVisible;
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
}