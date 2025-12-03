using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.MainPage.ScheduleCreation;

public class ScheduleCreationViewModel : ViewModelBase
{
    public event Action<ScheduleDto>? ScheduleCreated;

    private string scheduleName = string.Empty;
    public string ScheduleName
    {
        get => scheduleName;
        set => SetProperty(ref scheduleName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;

    public ICommand SaveCommand { get; }

    public ScheduleCreationViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(ScheduleName))
            return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();
        var schedule = new ScheduleDto { Name = ScheduleName };
        await service.AddSchedule(schedule);

        // Уведомляем всех подписчиков (например MainPageVM)
        ScheduleCreated?.Invoke(schedule);
        
        Console.WriteLine(Window);
        // Закрываем окно
        Window?.Close();
    }
}