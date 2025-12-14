using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

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

    public ICommand SaveCommand { get; }

    // Для создания нового
    public ScheduleEditorViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Создание расписания";
    }

    // Для редактирования существующего
    public ScheduleEditorViewModel(IServiceScopeFactory scopeFactory, ScheduleDto scheduleToEdit)
        : this(scopeFactory)
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
            schedule = await service.AddSchedule(createSchedule);
        }
        else
        {
            // Редактирование существующего
            schedule = new ScheduleDto { Id = editingSchedule.Id, Name = ScheduleName };
            await service.EditSchedule(schedule);
        }

        ScheduleSaved?.Invoke(schedule);
        Window?.Close();
    }
}