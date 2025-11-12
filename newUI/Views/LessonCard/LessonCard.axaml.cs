using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using newUI.ViewModels;

namespace newUI.Views.LessonCard;

public partial class LessonCard : UserControl
{
    public LessonCard() 
    {
        InitializeComponent();
    }
    
    public LessonCard(LessonCardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}