using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.MainPage.ScheduleEditor;

public class ScheduleEditorViewModel : ViewModelBase
{
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
    }

    // Для редактирования существующего
    public ScheduleEditorViewModel(IServiceScopeFactory scopeFactory, ScheduleDto scheduleToEdit)
        : this(scopeFactory)
    {
        editingSchedule = scheduleToEdit;
        ScheduleName = scheduleToEdit.Name;
    }

    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();

        ScheduleDto schedule;
        if (editingSchedule == null)
        {
            // Создание нового
            schedule = new ScheduleDto { Id = new Random().Next(1, 1000), Name = ScheduleName };
            await service.AddSchedule(schedule);
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