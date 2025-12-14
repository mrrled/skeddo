using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.Shared;

public class NotificationViewModel : ViewModelBase
{
    public string Message { get; }

    public ICommand CloseCommand { get; }

    public event Action? CloseRequested;

    public NotificationViewModel(string message)
    {
        Message = message;
        CloseCommand = new RelayCommand(() => CloseRequested?.Invoke());
    }
}