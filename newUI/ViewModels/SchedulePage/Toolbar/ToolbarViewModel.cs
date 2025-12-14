using System.Windows.Input;

namespace newUI.ViewModels.SchedulePage.Toolbar;

public class ToolbarViewModel(
    ICommand saveCommand,
    ICommand exportCommand,
    ICommand deleteCommand,
    ICommand closeCommand)
    : ViewModelBase
{
    public ICommand SaveCommand { get; } = saveCommand;
    public ICommand ExportCommand { get; } = exportCommand;
    public ICommand DeleteCommand { get; } = deleteCommand;
    public ICommand CloseCommand { get; } = closeCommand;
}