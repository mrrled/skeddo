using Application.Services;
using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(ITeacherService teacherService)
        {
            InitializeComponent();
            DataContext = new MainViewModel(teacherService); 
        }
    }
}