using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using newUI.Services;
using newUI.ViewModels.ClassroomsPage.ClassroomList;
using newUI.ViewModels.SchedulePage.Schedule;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;
using newUI.ViewModels.MainPage.ScheduleList;
using newUI.ViewModels.TeachersPage.TeacherList;

namespace newUI.ViewModels.Navigation;

public partial class NavigationBarViewModel(NavigationService nav) : ObservableObject
{
    [RelayCommand]
    private void NavigateMain()
        => nav.Navigate<ScheduleListViewModel>();
    
    [RelayCommand]
    private void NavigateScheduleTable()
        => nav.Navigate<ScheduleViewModel>();
    
    [RelayCommand]
    private void NavigateTeachers()
        => nav.Navigate<TeacherListViewModel>();

    [RelayCommand]
    private void NavigateSchoolSubjects()
        => nav.Navigate<SchoolSubjectListViewModel>();
    
    [RelayCommand]
    private void NavigateClassrooms()
        => nav.Navigate<ClassroomListViewModel>();
}