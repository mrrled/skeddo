using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchedulePage.LessonNumbers;

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

    public bool IsReadOnly => true;

    private readonly IServiceScopeFactory scopeFactory;
    private readonly LessonNumberDto? editingLessonNumber;
    private readonly Guid scheduleId;

    public ICommand SaveCommand { get; }

    // Создание
    public LessonNumberEditorViewModel(
        IServiceScopeFactory scopeFactory,
        int nextNumber,
        Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.scheduleId = scheduleId;

        LessonNumberNumber = nextNumber;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление номера занятия";
    }

    // Редактирование
    public LessonNumberEditorViewModel(
        IServiceScopeFactory scopeFactory,
        LessonNumberDto lessonNumberToEdit,
        Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.scheduleId = scheduleId;

        editingLessonNumber = lessonNumberToEdit;
        LessonNumberNumber = lessonNumberToEdit.Number;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Редактирование номера занятия";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();

        if (editingLessonNumber == null)
        {
            var newLessonNumber = new LessonNumberDto
            {
                Number = LessonNumberNumber
            };

            await service.AddLessonNumber(newLessonNumber, scheduleId);
            LessonNumberSaved?.Invoke(newLessonNumber);
        }
        else
        {
            var oldLessonNumber = editingLessonNumber;

            var newLessonNumber = new LessonNumberDto
            {
                Number = LessonNumberNumber,
                Time = oldLessonNumber.Time
            };

            await service.EditLessonNumber(oldLessonNumber, newLessonNumber, scheduleId);
            LessonNumberSaved?.Invoke(newLessonNumber);
        }

        Window?.Close();
    }
}