using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.ClassroomsPage.ClassroomEditor;

public class ClassroomEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<ClassroomDto>? ClassroomSaved;

    private string classroomName = string.Empty;

    public string ClassroomName
    {
        get => classroomName;
        set => SetProperty(ref classroomName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly ClassroomDto? editingClassroom;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public ClassroomEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление аудитории";
    }

    // Для редактирования существующего
    public ClassroomEditorViewModel(IServiceScopeFactory scopeFactory, ClassroomDto classroomToEdit)
        : this(scopeFactory)
    {
        editingClassroom = classroomToEdit;
        ClassroomName = classroomToEdit.Name;
        HeaderText = "Редактирование аудитории";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IClassroomServices>();

        ClassroomDto classroom;
        if (editingClassroom == null)
        {
            // Создание нового
            var createClassroom = new CreateClassroomDto { Name = ClassroomName };
            classroom = await service.AddClassroom(createClassroom);
        }
        else
        {
            // Редактирование существующего
            classroom = new ClassroomDto { Id = editingClassroom.Id, Name = ClassroomName };
            await service.EditClassroom(classroom);
        }

        ClassroomSaved?.Invoke(classroom);
        Window?.Close();
    }
}