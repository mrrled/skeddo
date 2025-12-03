using Avalonia.Controls;
using newUI.ViewModels.ClassroomsPage.ClassroomCreation;

namespace newUI.Views.ClassroomsPage.ClassroomCreation;

public partial class ClassroomCreationWindow : Window
{
    public ClassroomCreationWindow()
    {
        InitializeComponent();
    }
    
    private void NameTextBox_TextChanging(object? sender, TextChangingEventArgs e)
    {
        // Получаем ViewModel
        if (DataContext is ClassroomCreationViewModel viewModel)
        {
            // Вызываем асинхронный метод SetName
            if (sender is TextBox textBox)
            {
                // Не используйте await здесь, так как это обработчик события. 
                // Просто запускаем Task.
                viewModel.SetName(textBox.Text ?? string.Empty);
            }
        }
    }
}