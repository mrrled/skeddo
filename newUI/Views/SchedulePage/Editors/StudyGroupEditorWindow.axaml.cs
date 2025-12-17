using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.SchedulePage.Editors;

namespace newUI.Views.SchedulePage.Editors;

public partial class StudyGroupEditorWindow : Window
{
    public StudyGroupEditorWindow()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<StudyGroupEditorViewModel>();
    }
}