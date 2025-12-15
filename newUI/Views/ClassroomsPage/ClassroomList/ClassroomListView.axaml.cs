using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.ClassroomsPage.ClassroomList;

namespace newUI.Views.ClassroomsPage.ClassroomList;

public partial class ClassroomListView : UserControl
{
    public ClassroomListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<ClassroomListViewModel>();
    }
}