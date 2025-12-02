using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.TeachersPage.Teachers;

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
            var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();
            var lessonService = scope.ServiceProvider.GetRequiredService<ILessonServices>();
            await lessonService.AddLesson(new LessonDto()
            {
                Classroom = new ClassroomDto { Name = "514" },
                Id = 101,
                LessonNumber = new LessonNumberDto { Number = 1 },
                StudyGroup = new StudyGroupDto { Name = "ФТ-202" },
                Subject = new SchoolSubjectDto { Name = "дм" },
                Teacher = teacher,
            }, 1);
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