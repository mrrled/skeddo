using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.TeachersPage.TeacherList;

namespace newUI.Views.TeachersPage.TeacherList;

public partial class TeacherListView : UserControl
{
    public TeacherListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<TeacherListViewModel>();
    }
}