using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.TeachersPage.Teachers;

public class TeacherCreationViewModel : ViewModelBase
{
    private TeacherDto teacher = new();
    private ITeacherServices service;

    public TeacherDto Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    } 

    public TeacherCreationViewModel(ITeacherServices service)
    {
        this.service = service;
        var random = new Random();
        var id = random.Next(1, 1000);
        teacher.Id = id; 
        SaveChangesCommand = new RelayCommandAsync(SaveChanges);
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