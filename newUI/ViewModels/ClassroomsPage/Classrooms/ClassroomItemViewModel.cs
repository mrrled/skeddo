using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.ClassroomsPage.Classrooms;

public class ClassroomItemViewModel : ViewModelBase
{
    private ClassroomDto classroom;
    private IClassroomServices service;

    public ClassroomDto Classroom
    {
        get => classroom;
        set => SetProperty(ref classroom, value);
    }
}