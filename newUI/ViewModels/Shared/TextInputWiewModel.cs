using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.Shared;

public partial class TextInputViewModel : ViewModelBase
{
    private string? inputText;

    public string? InputText
    {
        get => inputText;
        set => SetProperty(ref inputText, value);
    }
    
    public string Title { get; }
    public string Prompt { get; }
    public string? Watermark { get; }
    
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    
    public TextInputViewModel(string title, string prompt, string? watermark = null)
    {
        Title = title;
        Prompt = prompt;
        Watermark = watermark;
        
        ConfirmCommand = new RelayCommand(() => { });
        CancelCommand = new RelayCommand(() => { InputText = null; });
    }
}