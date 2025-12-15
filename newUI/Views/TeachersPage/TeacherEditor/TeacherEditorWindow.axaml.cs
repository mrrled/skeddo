using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.TeachersPage.TeacherEditor;

namespace newUI.Views.TeachersPage.TeacherEditor;

public partial class TeacherEditorWindow : Window
{
    public TeacherEditorWindow()
    {
        InitializeComponent();

        // Берем ViewModel из DI-контейнера
        DataContext = App.Services.GetRequiredService<TeacherEditorViewModel>();
    }
}