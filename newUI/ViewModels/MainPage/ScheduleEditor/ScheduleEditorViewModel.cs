using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.MainPage.ScheduleEditor;

public class ScheduleEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<ScheduleDto>? ScheduleSaved;

    private string scheduleName = string.Empty;

    public string ScheduleName
    {
        get => scheduleName;
        set => SetProperty(ref scheduleName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;
    private readonly ScheduleDto? editingSchedule;
    private readonly IWindowManager windowManager;

    public ICommand SaveCommand { get; }

    // Для создания нового
    public ScheduleEditorViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Создание расписания";
    }

    // Для редактирования существующего
    public ScheduleEditorViewModel(IServiceScopeFactory scopeFactory, ScheduleDto scheduleToEdit, IWindowManager windowManager)
        : this(scopeFactory, windowManager)
    {
        editingSchedule = scheduleToEdit;
        ScheduleName = scheduleToEdit.Name;
        HeaderText = "Редактирование названия расписания";
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();

        ScheduleDto schedule;
        if (editingSchedule == null)
        {
            // Создание нового
            var createSchedule = new CreateScheduleDto { Name = ScheduleName };
            var scheduleResult = await service.AddSchedule(createSchedule);
            if (scheduleResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(scheduleResult.Error));
                return;
            }
            schedule = scheduleResult.Value;
        }
        else
        {
            // Редактирование существующего
            schedule = new ScheduleDto { Id = editingSchedule.Id, Name = ScheduleName };
            var scheduleEditResult = await service.EditSchedule(schedule);
            if (scheduleEditResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(scheduleEditResult.Error));
                return;
            }
        }

        ScheduleSaved?.Invoke(schedule);
        Window?.Close();
    }
}