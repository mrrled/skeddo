using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels;

namespace newUI;

public class NavigationService
{
    private readonly IServiceProvider _provider;

    public event Action<object>? CurrentViewModelChanged;

    public object? CurrentViewModel { get; private set; }

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Navigate<TViewModel>() where TViewModel : ViewModelBase
    {
        var vm = App.Services.GetRequiredService<TViewModel>();
        var viewType = ViewMappingService.GetViewType(typeof(TViewModel));

        if (typeof(UserControl).IsAssignableFrom(viewType))
        {
            var view = (UserControl)Activator.CreateInstance(viewType)!;
            view.DataContext = vm;
            CurrentViewModelChanged?.Invoke(vm);
        }
        else
        {
            throw new InvalidOperationException("Only UserControls can be shown in MainWindow ContentControl");
        }
    }
}