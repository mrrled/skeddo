using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.LessonNumbers;

public class LessonNumberEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<LessonNumberDto>? LessonNumberSaved;
    public event Action<LessonNumberDto>? LessonNumberDeleted;

    private int lessonNumberNumber;

    public int LessonNumberNumber
    {
        get => lessonNumberNumber;
        set => SetProperty(ref lessonNumberNumber, value);
    }

    public bool IsReadOnly => true;
    public bool IsEditMode => editingLessonNumber != null;
    
    private readonly IWindowManager windowManager;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly LessonNumberDto? editingLessonNumber;
    private readonly Guid scheduleId;

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    // ================== СОЗДАНИЕ ==================
    public LessonNumberEditorViewModel(
        IWindowManager windowManager,
        IServiceScopeFactory scopeFactory,
        int nextNumber,
        Guid scheduleId)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        this.scheduleId = scheduleId;

        LessonNumberNumber = nextNumber;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление номера занятия";
    }

    // ================== РЕДАКТИРОВАНИЕ ==================
    public LessonNumberEditorViewModel(
        IWindowManager windowManager,
        IServiceScopeFactory scopeFactory,
        LessonNumberDto lessonNumberToEdit,
        Guid scheduleId)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        this.scheduleId = scheduleId;

        editingLessonNumber = lessonNumberToEdit;
        LessonNumberNumber = lessonNumberToEdit.Number;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        DeleteCommand = new RelayCommandAsync(DeleteAsync);

        HeaderText = "Редактирование номера занятия";
    }

    // ================== SAVE ==================
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
            var newLessonNumber = new LessonNumberDto
            {
                Number = LessonNumberNumber,
                Time = editingLessonNumber.Time
            };

            await service.EditLessonNumber(editingLessonNumber, newLessonNumber, scheduleId);
            LessonNumberSaved?.Invoke(newLessonNumber);
        }

        Window?.Close();
    }

    // ================== DELETE ==================
    private async Task DeleteAsync()
    {
        if (editingLessonNumber == null)
            return;

        var confirmVm = new ConfirmDeleteViewModel(
            $"Вы уверены, что хотите удалить номер занятия {editingLessonNumber.Number}?"
        );

        var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

        if (result != true)
            return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonNumberServices>();

        await service.DeleteLessonNumber(editingLessonNumber, scheduleId);

        LessonNumberDeleted?.Invoke(editingLessonNumber);
        Window?.Close();
    }
}