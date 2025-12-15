using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.Shared;

public class ConfirmDeleteViewModel : ViewModelBase
{
    public string Title { get; }
    public string Message { get; }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public event Action<bool>? CloseRequested;

    public ConfirmDeleteViewModel(string message = "Вы уверены, что хотите удалить этот элемент?")
    {
        Title = "Окно подтверждения удаления";
        Message = message;

        ConfirmCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
        CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
    }
}