using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonCardViewModel : ViewModelBase
{
    private LessonDto lesson;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    public bool IsVisible { get; set; }
    public string Color { get; private set; }
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
        // StartDragCommand = new RelayCommand(StartDrag);
        // DropCommand = new RelayCommand<LessonCardViewModel>(OnDrop);
    }

    public LessonDto Lesson
    {
        get => lesson;
        set
        {
            if (SetProperty(ref lesson, value))
            {
                Color = WarningToColor(value.WarningType);
                OnPropertyChanged(nameof(Color));
            }
        }
    }
    
    // public ICommand StartDragCommand { get; }
    // public ICommand DropCommand { get; }
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
        var editVm = new LessonEditViewModel(scopeFactory, Lesson, !IsVisible);
        editVm.LessonUpdated += updatedLesson =>
        {
            Lesson = updatedLesson;
            OnPropertyChanged(nameof(Lesson));
            refreshCallback.Invoke();
        };
        
        windowManager.ShowDialog<LessonEditViewModel, LessonDto?>(editVm);
    }

    private string WarningToColor(WarningType warningType)
    {
        return warningType switch
        {
            WarningType.Conflict => "LightCoral",
            WarningType.Warning => "LemonChiffon",
            _ => "White"
        };
    }
}