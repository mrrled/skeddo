using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage;

namespace newUI.Views.MainPage;

public partial class ScheduleListViewView : UserControl
{
    public ScheduleListViewView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ScheduleListViewModel>();
    }
}