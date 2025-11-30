using Avalonia.Controls;
using newUI.ViewModels.TeachersPage.Teachers;

namespace newUI.Views.TeachersPage.TeacherListWindow;

public partial class TeacherListWindow : Window
{
    public TeacherListWindow(TeacherListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}