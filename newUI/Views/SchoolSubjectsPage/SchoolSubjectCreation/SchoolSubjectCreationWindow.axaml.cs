using Avalonia.Controls;
using newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

namespace newUI.Views.SchoolSubjectsPage.SchoolSubjectCreation;

public partial class SchoolSubjectCreationWindow : Window
{
    public SchoolSubjectCreationWindow()
    {
        InitializeComponent();
    }
    
    private void NameTextBox_TextChanging(object? sender, TextChangingEventArgs e)
    {
        // Получаем ViewModel
        if (DataContext is SchoolSubjectCreationViewModel viewModel)
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