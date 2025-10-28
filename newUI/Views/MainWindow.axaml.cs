using Application.Services;
using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel; // ViewModel внедряется через DI
        }
    }
}