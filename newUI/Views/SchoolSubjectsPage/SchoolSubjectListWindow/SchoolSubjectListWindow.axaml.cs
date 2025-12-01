using Avalonia.Controls;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

namespace newUI.Views.SchoolSubjectsPage.SchoolSubjectListWindow;

public partial class SchoolSubjectListWindow : Window
{
    public SchoolSubjectListWindow(SchoolSubjectListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}