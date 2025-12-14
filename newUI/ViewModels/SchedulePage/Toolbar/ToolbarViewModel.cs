using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.SchedulePage.Toolbar;

public class ToolbarViewModel : ViewModelBase
{
    public event Action? RequestPdfExport;
    public event Action? RequestExcelExport;

    private bool isEnabled;
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            if (SetProperty(ref isEnabled, value))
            {
                (PdfExportCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (ExcelExportCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.NotifyCanExecuteChanged();
                (CloseCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand PdfExportCommand { get; }
    public ICommand ExcelExportCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CloseCommand { get; }

    public ToolbarViewModel(
        ICommand saveCommand,
        ICommand deleteCommand,
        ICommand closeCommand)
    {
        SaveCommand = saveCommand;
        DeleteCommand = deleteCommand;
        CloseCommand = closeCommand;

        PdfExportCommand = new RelayCommand(
            () => RequestPdfExport?.Invoke(),
            () => IsEnabled);

        ExcelExportCommand = new RelayCommand(
            () => RequestExcelExport?.Invoke(),
            () => IsEnabled);
    }
}