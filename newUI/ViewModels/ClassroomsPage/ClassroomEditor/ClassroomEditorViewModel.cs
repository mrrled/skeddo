using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

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
    private readonly IWindowManager windowManager;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public ClassroomEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление аудитории";
        this.windowManager = windowManager;
    }

    // Для редактирования существующего
    public ClassroomEditorViewModel(IServiceScopeFactory scopeFactory, ClassroomDto classroomToEdit, IWindowManager windowManager)
        : this(scopeFactory, windowManager)
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
            var classroomResult = await service.AddClassroom(createClassroom);
            if (classroomResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(classroomResult.Error));
                return;
            }
            classroom = classroomResult.Value;
        }
        else
        {
            // Редактирование существующего
            classroom = new ClassroomDto { Id = editingClassroom.Id, Name = ClassroomName };
            var classroomEdit = await service.EditClassroom(classroom);
            if (classroomEdit.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(classroomEdit.Error));
                return;
            }
        }

        ClassroomSaved?.Invoke(classroom);
        Window?.Close();
    }
}