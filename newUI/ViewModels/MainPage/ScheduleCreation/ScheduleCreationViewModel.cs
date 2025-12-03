using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.MainPage.ScheduleCreation;

public class ScheduleCreationViewModel : ViewModelBase
{
    private string scheduleName = string.Empty;

    public string ScheduleName
    {
        get => scheduleName;
        set => SetProperty(ref scheduleName, value);
    }

    private readonly IServiceScopeFactory scopeFactory;

    public ScheduleCreationViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveCommand = new RelayCommandAsync(SaveAsync);
    }

    public ICommand SaveCommand { get; }

    public event Action<ScheduleDto>? ScheduleCreated;

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(ScheduleName))
            return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IScheduleServices>();

        var schedule = new ScheduleDto { Name = ScheduleName };
        await service.AddSchedule(schedule);

        // уведомляем подписчиков
        ScheduleCreated?.Invoke(schedule);

        // закрываем окно
        if (App.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            foreach (var window in desktop.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}