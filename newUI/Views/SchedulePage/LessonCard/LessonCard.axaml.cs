using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views.SchedulePage.LessonCard;

public partial class LessonCard : UserControl
{
    public LessonCard() 
    {
        InitializeComponent();
    }
    
    // public LessonCard(LessonCardViewModel viewModel)
    // {
    //     InitializeComponent();
    //     DataContext = viewModel;
    //     viewModel.Height = Cell.Height;
    //     viewModel.Width = Cell.Width;
    // }
}