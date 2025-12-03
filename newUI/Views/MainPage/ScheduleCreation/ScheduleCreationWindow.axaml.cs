using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage.ScheduleCreation;

namespace newUI.Views.MainPage.ScheduleCreation;

public partial class ScheduleCreationWindow : Window
{
    public ScheduleCreationWindow()
    {
        InitializeComponent();

        // Берем ViewModel из DI-контейнера
        DataContext = App.Services.GetRequiredService<ScheduleCreationViewModel>();
    }
}