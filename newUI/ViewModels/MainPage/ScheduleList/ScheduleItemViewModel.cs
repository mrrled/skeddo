using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Application.DtoModels;

namespace newUI.ViewModels.MainPage.ScheduleList;

public class ScheduleItemViewModel : ObservableObject
{
    public event Action<ScheduleItemViewModel>? RequestDelete;
    public event Action<ScheduleItemViewModel>? RequestEdit;

    public ScheduleDto Schedule { get; }

    public string Name
    {
        get => Schedule.Name;
        set
        {
            if (Schedule.Name == value)
                return;
            Schedule.Name = value;
            OnPropertyChanged();
        }
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public ScheduleItemViewModel(ScheduleDto schedule)
    {
        Schedule = schedule;
        EditCommand = new RelayCommand(() => RequestEdit?.Invoke(this));
        DeleteCommand = new RelayCommand(() => RequestDelete?.Invoke(this));
    }
}