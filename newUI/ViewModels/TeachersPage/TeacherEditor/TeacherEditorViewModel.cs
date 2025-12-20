using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.TeachersPage.TeacherEditor;

public class TeacherEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
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
    private readonly IWindowManager windowManager;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public TeacherEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление преподавателя";
    }

    // Для редактирования существующего
    public TeacherEditorViewModel(IServiceScopeFactory scopeFactory, TeacherDto teacherToEdit, IWindowManager windowManager)
        : this(scopeFactory, windowManager)
    {
        editingTeacher = teacherToEdit;
        TeacherSurname = teacherToEdit.Surname;
        TeacherName = teacherToEdit.Name;
        teacherPatronymic = teacherToEdit.Patronymic;
        HeaderText = "Редактирование преподавателя";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITeacherServices>();

        TeacherDto teacher;
        if (editingTeacher == null)
        {
            // Создание нового
            var createTeacher = new CreateTeacherDto
            {
                Surname = TeacherSurname,
                Name = TeacherName,
                Patronymic = TeacherPatronymic
            };
            var teacherResult = await service.AddTeacher(createTeacher);
            if (teacherResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(teacherResult.Error));
                return;
            }
            teacher = teacherResult.Value;
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
            var teacherEditResult = await service.EditTeacher(teacher);
            if (teacherEditResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(teacherEditResult.Error));
                return;
            }
        }

        TeacherSaved?.Invoke(teacher);
        Window?.Close();
    }
}