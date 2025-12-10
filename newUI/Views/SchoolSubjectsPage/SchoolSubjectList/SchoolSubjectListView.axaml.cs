using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectList;

namespace newUI.Views.SchoolSubjectsPage.SchoolSubjectList;

public partial class SchoolSubjectListView : UserControl
{
    public SchoolSubjectListView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<SchoolSubjectListViewModel>();
    }
}