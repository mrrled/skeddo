using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Application.DtoModels;
using newUI.Services;

namespace newUI.ViewModels.MainPage.ScheduleList;

public class ScheduleItemViewModel : ObservableObject
{
    private bool isEditing = false;
    public bool IsEditing
    {
        get => isEditing;
        set => SetProperty(ref isEditing, value);
    }

    public ScheduleDto Schedule { get; }
    public string Name
    {
        get => Schedule.Name;
        set => Schedule.Name = value;
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly IWindowManager windowManager;

    public ScheduleItemViewModel(ScheduleDto schedule, IWindowManager windowManager)
    {
        Schedule = schedule;
        this.windowManager = windowManager;

        EditCommand = new RelayCommand(Edit);
        DeleteCommand = new RelayCommand(Delete);
    }

    private void Edit()
    {
        IsEditing = !IsEditing;
    }

    private void Delete()
    {
        // Можно добавить логику удаления через сервис
    }
}