using Avalonia.Controls;
using newUI.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.Services;

public class WindowManager : IWindowManager
{
    private readonly IServiceScopeFactory scopeFactory;
    
    public WindowManager(IServiceScopeFactory scopeFactory) 
    {
        this.scopeFactory = scopeFactory;
    }
    
    private Window CreateWindowInstance<TViewModel>() 
        where TViewModel : ViewModelBase
    {
        var viewType = ViewMappingService.GetViewType(typeof(TViewModel));
        var window = (Window)Activator.CreateInstance(viewType)!;
        
        return window;
    }
    
    public void Show<TViewModel>() where TViewModel : ViewModelBase
    {
        var window = CreateWindowInstance<TViewModel>();
        using (var scope = scopeFactory.CreateScope())
        {
            window.DataContext = scope.ServiceProvider.GetRequiredService<TViewModel>();
        }

        window.Show();
    }

    public async Task<TResult?> ShowDialog<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : ViewModelBase
    {
        var dialog = CreateWindowInstance<TViewModel>();
        dialog.DataContext = viewModel;
        var owner = GetActiveWindow();
        
        if (owner == null)
        {
            throw new InvalidOperationException("Не найдено активное главное окно для модального диалога.");
        }

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