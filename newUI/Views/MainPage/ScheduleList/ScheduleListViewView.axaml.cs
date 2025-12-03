using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage;

namespace newUI.Views.MainPage.ScheduleList;

public partial class ScheduleListViewView : UserControl
{
    public ScheduleListViewView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ScheduleListViewModel>();
    }
}