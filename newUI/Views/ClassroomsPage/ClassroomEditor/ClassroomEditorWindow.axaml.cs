using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.ClassroomsPage.ClassroomEditor;

namespace newUI.Views.ClassroomsPage.ClassroomEditor;

public partial class ClassroomEditorWindow : Window
{
    public ClassroomEditorWindow()
    {
        InitializeComponent();

        // Берем ViewModel из DI-контейнера
        DataContext = App.Services.GetRequiredService<ClassroomEditorViewModel>();
    }
}