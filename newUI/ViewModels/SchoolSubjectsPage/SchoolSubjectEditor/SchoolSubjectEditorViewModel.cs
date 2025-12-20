using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectEditor;

public class SchoolSubjectEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<SchoolSubjectDto>? SchoolSubjectSaved;

    private string schoolSubjectName = string.Empty;

    public string SchoolSubjectName
    {
        get => schoolSubjectName;
        set => SetProperty(ref schoolSubjectName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly SchoolSubjectDto? editingSchoolSubject;
    private readonly IWindowManager windowManager;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public SchoolSubjectEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление предмета";
    }

    // Для редактирования существующего
    public SchoolSubjectEditorViewModel(IServiceScopeFactory scopeFactory, SchoolSubjectDto schoolSubjectToEdit, IWindowManager windowManager)
        : this(scopeFactory, windowManager)
    {
        editingSchoolSubject = schoolSubjectToEdit;
        SchoolSubjectName = schoolSubjectToEdit.Name;
        HeaderText = "Редактирование предмета";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();

        SchoolSubjectDto schoolSubject;
        if (editingSchoolSubject == null)
        {
            // Создание нового
            var createSchoolSubject = new CreateSchoolSubjectDto { Name = SchoolSubjectName };
            var schoolSubjectResult = await service.AddSchoolSubject(createSchoolSubject);
            if (schoolSubjectResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(schoolSubjectResult.Error));
                return;
            }
            schoolSubject = schoolSubjectResult.Value;
        }
        else
        {
            // Редактирование существующего
            schoolSubject = new SchoolSubjectDto { Id = editingSchoolSubject.Id, Name = SchoolSubjectName };
            var schoolSubjectEditResult = await service.EditSchoolSubject(schoolSubject);
            if (schoolSubjectEditResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(schoolSubjectEditResult.Error));
                return;
            }
        }

        SchoolSubjectSaved?.Invoke(schoolSubject);
        Window?.Close();
    }
}