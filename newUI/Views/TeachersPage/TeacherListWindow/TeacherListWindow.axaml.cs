using Avalonia.Controls;
using newUI.ViewModels.Teachers;

namespace newUI.Views.TeachersPage.TeacherListWindow;

public partial class TeacherListWindow : Window
{
    public TeacherListWindow(TeacherListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}