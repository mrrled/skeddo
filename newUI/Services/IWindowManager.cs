using System;
using newUI.ViewModels;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.Services;

public interface IWindowManager
{
    Window ShowWindow<TViewModel>(TViewModel viewModel) where TViewModel : ViewModelBase;

    Task<TResult?> ShowDialog<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : ViewModelBase;
}