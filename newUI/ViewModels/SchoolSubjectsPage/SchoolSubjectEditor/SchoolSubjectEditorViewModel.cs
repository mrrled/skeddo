using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

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

    public ICommand SaveCommand { get; }

    // Для создания нового
    public SchoolSubjectEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление предмета";
    }

    // Для редактирования существующего
    public SchoolSubjectEditorViewModel(IServiceScopeFactory scopeFactory, SchoolSubjectDto schoolSubjectToEdit)
        : this(scopeFactory)
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
            schoolSubject = new SchoolSubjectDto { Id = new Random().Next(1, 1000), Name = SchoolSubjectName };
            await service.AddSchoolSubject(schoolSubject);
        }
        else
        {
            // Редактирование существующего
            schoolSubject = new SchoolSubjectDto { Id = editingSchoolSubject.Id, Name = SchoolSubjectName };
            await service.EditSchoolSubject(schoolSubject);
        }

        SchoolSubjectSaved?.Invoke(schoolSubject);
        Window?.Close();
    }
}