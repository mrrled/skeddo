using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.Editors;

public class LessonNumberEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<LessonNumberDto>? LessonNumberSaved;

    private int lessonNumberNumber;

    public int LessonNumberNumber
    {
        get => lessonNumberNumber;
        set => SetProperty(ref lessonNumberNumber, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly LessonNumberDto? editingLessonNumber;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public LessonNumberEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление учебной группы";
    }

    // Для редактирования существующего
    public LessonNumberEditorViewModel(IServiceScopeFactory scopeFactory, LessonNumberDto lessonNumberToEdit)
        : this(scopeFactory)
    {
        editingLessonNumber = lessonNumberToEdit;
        LessonNumberNumber = lessonNumberToEdit.Number;
        HeaderText = "Редактирование учебной группы";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();

        LessonNumberDto lessonNumber;
        if (editingLessonNumber == null)
        {
            // // Создание нового
            // var createLessonNumber = new CreateLessonNumberDto { Number = LessonNumberNumber };
            // lessonNumber = await service.AddLessonNumber(createLessonNumber);
            
            // временно
            lessonNumber = new LessonNumberDto { Number = LessonNumberNumber };
            // временно
        }
        else
        {
            // // Редактирование существующего
            // lessonNumber = new LessonNumberDto { Id = editingLessonNumber.Id, Number = LessonNumberNumber };
            // await service.EditLessonNumber(lessonNumber);
            
            // временно
            lessonNumber = new LessonNumberDto { Id = editingLessonNumber.Id, Number = LessonNumberNumber };
            // временно
        }

        LessonNumberSaved?.Invoke(lessonNumber);
        Window?.Close();
    }
}