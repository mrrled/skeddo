using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.TeachersPage.Teachers;

public class TeacherItemViewModel : ViewModelBase
{
    private TeacherDto teacher;
    private ITeacherServices service;

    public TeacherDto Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    }
}