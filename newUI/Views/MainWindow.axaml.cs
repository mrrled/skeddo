using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); 
        }
    }
}