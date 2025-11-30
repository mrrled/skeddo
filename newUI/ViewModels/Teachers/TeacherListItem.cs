using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using newUI.ViewModels.Helpers;

namespace newUI.ViewModels.Teachers;

public class TeacherListItem : ViewModelBase
{
    private TeacherDto teacher;
    private ITeacherServices service;

    public TeacherDto Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    }
}