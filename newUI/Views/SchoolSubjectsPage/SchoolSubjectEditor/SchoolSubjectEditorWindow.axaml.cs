using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectEditor;

namespace newUI.Views.SchoolSubjectsPage.SchoolSubjectEditor;

public partial class SchoolSubjectEditorWindow : Window
{
    public SchoolSubjectEditorWindow()
    {
        InitializeComponent();

        // Берем ViewModel из DI-контейнера
        DataContext = App.Services.GetRequiredService<SchoolSubjectEditorViewModel>();
    }
}