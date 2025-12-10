using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using newUI.ViewModels.Shared;

namespace newUI.Views.Shared;

public partial class ConfirmDeleteWindow : Window
{
    public ConfirmDeleteWindow()
    {
        InitializeComponent();

        // Подписка на событие закрытия после того, как DataContext установлен
        Opened += (s, e) =>
        {
            if (DataContext is ConfirmDeleteViewModel vm)
            {
                vm.CloseRequested += result =>
                {
                    Close(result); // Закрывает окно и возвращает результат ShowDialog
                };
            }
        };
    }
}