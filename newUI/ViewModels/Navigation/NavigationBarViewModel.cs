using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using newUI.ViewModels.SchedulePage.Schedule;
using newUI.ViewModels.TeachersPage.Teachers;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

namespace newUI.ViewModels.Navigation;

public partial class NavigationBarViewModel(NavigationService nav) : ObservableObject
{
    [RelayCommand]
    private void NavigateTeachers()
        => nav.Navigate<TeacherListViewModel>();

    [RelayCommand]
    private void NavigateSchoolSubjects()
        => nav.Navigate<SchoolSubjectListViewModel>();
    
    [RelayCommand]
    private void NavigateScheduleTable()
        => nav.Navigate<ScheduleViewModel>();
}