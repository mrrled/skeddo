using Avalonia.Controls;
using newUI.ViewModels;

namespace newUI.Views.TeacherCreationWindow;

public partial class TeacherCreationWindow : Window
{
    public TeacherCreationWindow()
    {
        InitializeComponent();
    }
    
    private void NameTextBox_TextChanging(object? sender, TextChangingEventArgs e)
    {
        // Получаем ViewModel
        if (DataContext is TeacherCreationViewModel viewModel)
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
    
    private void SurnameTextBox_TextChanging(object? sender, TextChangingEventArgs e)
    {
        if (DataContext is TeacherCreationViewModel viewModel)
        {
            if (sender is TextBox textBox)
            {
                viewModel.SetSurname(textBox.Text ?? string.Empty);
            }
        }
    }
    
    private void PatronymicTextBox_TextChanging(object? sender, TextChangingEventArgs e)
    {
        if (DataContext is TeacherCreationViewModel viewModel)
        {
            if (sender is TextBox textBox)
            {
                viewModel.SetPatronymic(textBox.Text ?? string.Empty);
            }
        }
    }
}