using System;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.MainPage.ScheduleList;

public class ScheduleItemViewModel : ViewModelBase
{
    private string name;
    public string Name
    {
        get => name;
        set => SetProperty(ref name, value);
    }

    private bool isEditing;
    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }

    public ScheduleItemViewModel(string name)
    {
        Name = name;
    }

    public IRelayCommand EditCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public event EventHandler? Deleted;

    public ScheduleItemViewModel()
    {
        EditCommand = new RelayCommand(Edit);
        DeleteCommand = new RelayCommand(Delete);
    }

    private void Edit()
    {
        IsEditing = !IsEditing;
    }

    private void Delete()
    {
        Deleted?.Invoke(this, EventArgs.Empty);
    }
}