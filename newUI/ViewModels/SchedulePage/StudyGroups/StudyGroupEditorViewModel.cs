using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.StudyGroups;

public class StudyGroupEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<StudyGroupDto>? StudyGroupSaved;
    public event Action<StudyGroupDto>? StudyGroupDeleted;

    private string studyGroupName = string.Empty;
    public string StudyGroupName
    {
        get => studyGroupName;
        set => SetProperty(ref studyGroupName, value);
    }

    public bool IsEditMode => editingStudyGroup != null;

    private readonly IWindowManager windowManager;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly StudyGroupDto? editingStudyGroup;

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    // ================== СОЗДАНИЕ ==================
    public StudyGroupEditorViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление учебной группы";
    }

    // ================== РЕДАКТИРОВАНИЕ ==================
    public StudyGroupEditorViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory, StudyGroupDto studyGroupToEdit)
        : this(windowManager, scopeFactory)
    {
        editingStudyGroup = studyGroupToEdit;
        StudyGroupName = studyGroupToEdit.Name;

        DeleteCommand = new RelayCommandAsync(DeleteAsync);
        HeaderText = "Редактирование учебной группы";
    }

    // ================== SAVE ==================
    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();

        StudyGroupDto studyGroup;

        if (editingStudyGroup == null)
        {
            var createStudyGroup = new CreateStudyGroupDto { Name = StudyGroupName };
            studyGroup = (await service.AddStudyGroup(createStudyGroup)).Value; //TODO: показ ошибки
        }
        else
        {
            studyGroup = new StudyGroupDto
            {
                Id = editingStudyGroup.Id,
                Name = StudyGroupName
            };

            await service.EditStudyGroup(studyGroup);
        }

        StudyGroupSaved?.Invoke(studyGroup);
        Window?.Close();
    }

    // ================== DELETE ==================
    private async Task DeleteAsync()
    {
        if (editingStudyGroup == null)
            return;

        var confirmVm = new ConfirmDeleteViewModel(
            $"Вы уверены, что хотите удалить учебную группу \"{editingStudyGroup.Name}\"?"
        );

        var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

        if (result != true)
            return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();

        await service.DeleteStudyGroup(editingStudyGroup);

        StudyGroupDeleted?.Invoke(editingStudyGroup);
        Window?.Close();
    }
}
