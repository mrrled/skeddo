using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views.MainWindow;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel; // ViewModel внедряется через DI
    }
}