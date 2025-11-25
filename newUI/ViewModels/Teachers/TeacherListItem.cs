using System.Linq;
using Application.DtoModels;
using Application.Services;
using Avalonia.Collections;
using newUI.ViewModels.Helpers;

namespace newUI.ViewModels.ObjectList;

public class TeacherListItem : ViewModelBase, IObjectListItem<TeacherListItem>
{
    private TeacherDto teacher;

    public TeacherDto Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    }
        
    // public static AvaloniaList<TeacherListItem> FetchFromBackend(IService service)
    // {
    //     var teachers = service.FetchTeachersFromBackendAsync().Result;
    //     return new AvaloniaList<TeacherListItem>(
    //         teachers.Select(teacher => new TeacherListItem{ Teacher = teacher })
    //     );
    // }
}