using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using newUI.ViewModels.MainPage;

namespace newUI.Views.MainPage;

public partial class MainPageView : UserControl
{
    public MainPageView()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<MainPageViewModel>();
    }
}