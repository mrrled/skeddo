using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.SchedulePage.LessonNumbers;

namespace newUI.Views.SchedulePage.Editors;

public partial class LessonNumberEditorWindow : Window
{
    public LessonNumberEditorWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<LessonNumberEditorViewModel>();
    }
}