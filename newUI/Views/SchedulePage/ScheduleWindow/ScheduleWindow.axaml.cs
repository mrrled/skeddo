using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using newUI.ViewModels.SchedulePage.Schedule;

namespace newUI.Views.SchedulePage.ScheduleWindow;

public partial class ScheduleWindow : Window
{
    public ScheduleWindow(ScheduleViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}