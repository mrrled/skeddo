using Avalonia.Controls;
using newUI.ViewModels;
using System;
using System.Collections.Generic;

namespace newUI.Services;


public static class ViewMappingService
{
    private static readonly Dictionary<Type, Type> Mappings = new();
    
    public static void Register<TViewModel, TView>() 
        where TViewModel : ViewModelBase
        where TView : Window
    {
        var viewModelType = typeof(TViewModel);
        var viewType = typeof(TView);
        
        if (!Mappings.TryAdd(viewModelType, viewType))
        {
            throw new InvalidOperationException($"ViewModel {viewModelType.Name} уже зарегистрирована.");
        }
    }

    public static Type GetViewType(Type viewModelType)
    {
        if (Mappings.TryGetValue(viewModelType, out var viewType))
        {
            return viewType;
        }

        throw new InvalidOperationException(
            $"View wasn't found for ViewModel of type: {viewModelType.FullName}. " +
            "Make sure it's registered at ViewMappingService."); //регистрировать нужно в app.axaml.cs
    }
}