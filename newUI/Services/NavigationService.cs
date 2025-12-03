using System;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels;

namespace newUI.Services;

public class NavigationService
{
    private readonly IServiceProvider _provider;

    public event Action? CurrentViewModelChanged;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke();
        }
    }

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void Navigate<TViewModel>() where TViewModel : ViewModelBase
    {
        var vm = _provider.GetRequiredService<TViewModel>();
        CurrentViewModel = vm;
    }
}