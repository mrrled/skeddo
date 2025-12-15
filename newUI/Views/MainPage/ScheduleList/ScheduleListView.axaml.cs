using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage.ScheduleList;

namespace newUI.Views.MainPage.ScheduleList;

public partial class ScheduleListView : UserControl
{
    public ScheduleListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ScheduleListViewModel>();
    }
}