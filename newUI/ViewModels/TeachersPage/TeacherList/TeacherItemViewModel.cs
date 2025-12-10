using System;
using System.Windows.Input;
using Application.DtoModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.TeachersPage.TeacherList;

public class TeacherItemViewModel : ObservableObject
{
    public event Action<TeacherItemViewModel>? RequestDelete;
    public event Action<TeacherItemViewModel>? RequestEdit;

    public TeacherDto Teacher { get; }

    public string Surname
    {
        get => Teacher.Surname;
        set
        {
            if (Teacher.Surname == value)
                return;
            Teacher.Surname = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => Teacher.Name;
        set
        {
            if (Teacher.Name == value)
                return;
            Teacher.Name = value;
            OnPropertyChanged();
        }
    }

    public string Patronymic
    {
        get => Teacher.Patronymic;
        set
        {
            if (Teacher.Patronymic == value)
                return;
            Teacher.Patronymic = value;
            OnPropertyChanged();
        }
    }

    public string FullName => $"{Surname} {Name} {Patronymic}";

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public TeacherItemViewModel(TeacherDto teacher)
    {
        Teacher = teacher;
        EditCommand = new RelayCommand(() => RequestEdit?.Invoke(this));
        DeleteCommand = new RelayCommand(() => RequestDelete?.Invoke(this));
    }
}