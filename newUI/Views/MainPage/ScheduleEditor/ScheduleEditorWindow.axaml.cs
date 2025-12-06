using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage.ScheduleEditor;

namespace newUI.Views.MainPage.ScheduleEditor;

public partial class ScheduleEditorWindow : Window
{
    public ScheduleEditorWindow()
    {
        InitializeComponent();

        // Берем ViewModel из DI-контейнера
        DataContext = App.Services.GetRequiredService<ScheduleEditorViewModel>();
    }
}