using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.SchedulePage.StudySubgroups;

public class StudySubgroupCreatorViewModel : ViewModelBase
{
    public string Title { get; }
    public string Prompt { get; }
    public string? Watermark { get; }

    private string? _inputText;
    public string? InputText
    {
        get => _inputText;
        set => SetProperty(ref _inputText, value);
    }

    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }

    public StudySubgroupCreatorViewModel()
    {
        Title = "Добавление подгруппы";

        // Команда подтверждения
        ConfirmCommand = new RelayCommand(() =>
        {
            // Используем свойство Window, которое ваш WindowManager установил сам
            if (Window is Avalonia.Controls.Window avaloniaWindow)
            {
                // Метод Close(результат) закрывает окно и возвращает значение 
                // в await windowManager.ShowDialog<..., TResult>()
                avaloniaWindow.Close(InputText);
            }
        });

        // Команда отмены
        CancelCommand = new RelayCommand(() =>
        {
            if (Window is Avalonia.Controls.Window avaloniaWindow)
            {
                avaloniaWindow.Close(null); // Возвращаем null
            }
        });
    }
}