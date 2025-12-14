using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using newUI.ViewModels.Shared;

namespace newUI.Views.Shared;

public partial class NotificationWindow : Window
{
    public NotificationWindow()
    {
        InitializeComponent();

        Opened += (s, e) =>
        {
            if (DataContext is NotificationViewModel vm)
            {
                vm.CloseRequested += () =>
                {
                    Close();
                };
            }
        };
    }
}