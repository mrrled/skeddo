using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.Services;

namespace newUI.ViewModels;

public class TeacherCreationViewModel : ViewModelBase
{
    private DtoTeacher teacher = new();
    private IService service;

    public DtoTeacher Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    } 

    public TeacherCreationViewModel(IService service)
    {
        this.service = service;
        SaveChangesCommand = new AsyncRelayCommand(SaveChanges);
    }
    
    public ICommand SaveChangesCommand { get; set; }

    public Task SaveChanges()
    {
        service.AddTeacher(teacher);
        return Task.CompletedTask;
    }
    public Task SetName(string name)
    {
        teacher.Name = name;
        return Task.CompletedTask;
    }

    public Task SetSurname(string surname) 
    {
        teacher.Surname = surname;
        return Task.CompletedTask;
    }
    
    public Task SetPatronymic(string patronymic)
    {
        teacher.Patronymic = patronymic;
        return Task.CompletedTask;
    }
}