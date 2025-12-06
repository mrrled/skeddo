using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.TeachersPage.TeacherCreation;

public class TeacherCreationViewModel : ViewModelBase
{
    private TeacherDto teacher = new();
    private readonly IServiceScopeFactory scopeFactory;

    public TeacherDto Teacher
    {
        get => teacher;
        set => SetProperty(ref teacher, value);
    } 

    public TeacherCreationViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        var random = new Random();
        var id = random.Next(1, 1000);
        teacher.Id = id; 
        SaveChangesCommand = new RelayCommandAsync(SaveChanges);
    }
    
    public ICommand SaveChangesCommand { get; set; }

    public async Task SaveChanges()
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
            await scheduleService.GetScheduleByIdAsync(1);
            var ser = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            await ser.AddLesson(new LessonDto
            {
                Classroom = new ClassroomDto { Id = 1, Name = "513" },
                Id = 5,
                LessonNumber = new LessonNumberDto { Id = 1, Number = 1 },
                SchoolSubject = new SchoolSubjectDto { Id = 2, Name = "алгем" },
                StudyGroup = new StudyGroupDto { Id = 1, Name = "ФТ-202" },
                Teacher = null,
                WarningType = WarningType.Normal
            }, 1);
            var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
            await service.AddTeacher(teacher);
        } 
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