using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.TeachersPage.TeacherEditor;

public class TeacherEditorViewModel : ViewModelBase
{
    public event Action<TeacherDto>? TeacherSaved;

    private string teacherSurname = string.Empty;

    public string TeacherSurname
    {
        get => teacherSurname;
        set => SetProperty(ref teacherSurname, value);
    }

    private string teacherName = string.Empty;

    public string TeacherName
    {
        get => teacherName;
        set => SetProperty(ref teacherName, value);
    }

    private string teacherPatronymic = string.Empty;

    public string TeacherPatronymic
    {
        get => teacherPatronymic;
        set => SetProperty(ref teacherPatronymic, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly TeacherDto? editingTeacher;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public TeacherEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
    }

    // Для редактирования существующего
    public TeacherEditorViewModel(IServiceScopeFactory scopeFactory, TeacherDto teacherToEdit)
        : this(scopeFactory)
    {
        editingTeacher = teacherToEdit;
        TeacherSurname = teacherToEdit.Surname;
        TeacherName = teacherToEdit.Name;
        teacherPatronymic = teacherToEdit.Patronymic;
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();

        TeacherDto teacher;
        if (editingTeacher == null)
        {
            // Создание нового
            teacher = new TeacherDto
            {
                Id = new Random().Next(1, 1000),
                Surname = TeacherSurname,
                Name = TeacherName,
                Patronymic = TeacherPatronymic
            };
            await service.AddTeacher(teacher);
        }
        else
        {
            // Редактирование существующего
            teacher = new TeacherDto
            {
                Id = editingTeacher.Id,
                Surname = TeacherSurname,
                Name = TeacherName,
                Patronymic = TeacherPatronymic
            };
            await service.EditTeacher(teacher);
        }

        TeacherSaved?.Invoke(teacher);
        Window?.Close();
    }
}