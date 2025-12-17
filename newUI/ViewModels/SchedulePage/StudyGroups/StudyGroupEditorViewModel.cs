using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.StudyGroups;

public class StudyGroupEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<StudyGroupDto>? StudyGroupSaved;

    private string studyGroupName = string.Empty;

    public string StudyGroupName
    {
        get => studyGroupName;
        set => SetProperty(ref studyGroupName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly StudyGroupDto? editingStudyGroup;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public StudyGroupEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление учебной группы";
    }

    // Для редактирования существующего
    public StudyGroupEditorViewModel(IServiceScopeFactory scopeFactory, StudyGroupDto studyGroupToEdit)
        : this(scopeFactory)
    {
        editingStudyGroup = studyGroupToEdit;
        StudyGroupName = studyGroupToEdit.Name;
        HeaderText = "Редактирование учебной группы";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();

        StudyGroupDto studyGroup;
        if (editingStudyGroup == null)
        {
            // Создание нового
            var createStudyGroup = new CreateStudyGroupDto { Name = StudyGroupName };
            studyGroup = await service.AddStudyGroup(createStudyGroup);
        }
        else
        {
            // Редактирование существующего
            studyGroup = new StudyGroupDto { Id = editingStudyGroup.Id, Name = StudyGroupName };
            await service.EditStudyGroup(studyGroup);
        }

        StudyGroupSaved?.Invoke(studyGroup);
        Window?.Close();
    }
}