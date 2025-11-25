using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using newUI.ViewModels;
using newUI.ViewModels.Teachers;

namespace newUI.Views.TeacherListWindow;

public partial class TeacherListWindow : Window
{
    public TeacherListWindow(TeacherListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}