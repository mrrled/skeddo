using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels;

public class WindowManager : IWindowManager
{
    private readonly IServiceScopeFactory scopeFactory;

    public WindowManager(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
    }

    private Window CreateWindowInstance<TViewModel>() where TViewModel : ViewModelBase
    {
        var viewType = ViewMappingService.GetViewType(typeof(TViewModel));
        var window = (Window)Activator.CreateInstance(viewType)!;
        return window;
    }

    // Новый метод: открываем окно с готовой VM
    public void ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase
    {
        var window = CreateWindowInstance<TViewModel>();
        window.DataContext = viewModel;
        window.Show();
    }

    public async Task<TResult?> ShowDialog<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : ViewModelBase
    {
        var dialog = CreateWindowInstance<TViewModel>();
        dialog.DataContext = viewModel;
        var owner = GetActiveWindow();

        if (owner == null)
            throw new InvalidOperationException("Не найдено активное главное окно для модального диалога.");

        return await dialog.ShowDialog<TResult?>(owner);
    }

    private static Window? GetActiveWindow()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }
}