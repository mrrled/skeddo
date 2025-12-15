using System;
using System.Windows.Input;
using Application.DtoModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.ClassroomsPage.ClassroomList;

public class ClassroomItemViewModel : ObservableObject
{
    public event Action<ClassroomItemViewModel>? RequestDelete;
    public event Action<ClassroomItemViewModel>? RequestEdit;

    public ClassroomDto Classroom { get; }

    public string Name
    {
        get => Classroom.Name;
        set
        {
            if (Classroom.Name == value)
                return;
            Classroom.Name = value;
            OnPropertyChanged();
        }
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public ClassroomItemViewModel(ClassroomDto classroom)
    {
        Classroom = classroom;
        EditCommand = new RelayCommand(() => RequestEdit?.Invoke(this));
        DeleteCommand = new RelayCommand(() => RequestDelete?.Invoke(this));
    }
}