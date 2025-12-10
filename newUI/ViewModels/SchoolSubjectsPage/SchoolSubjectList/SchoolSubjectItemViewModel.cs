using System;
using System.Windows.Input;
using Application.DtoModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectList;

public class SchoolSubjectItemViewModel : ObservableObject
{
    public event Action<SchoolSubjectItemViewModel>? RequestDelete;
    public event Action<SchoolSubjectItemViewModel>? RequestEdit;

    public SchoolSubjectDto SchoolSubject { get; }

    public string Name
    {
        get => SchoolSubject.Name;
        set
        {
            if (SchoolSubject.Name == value)
                return;
            SchoolSubject.Name = value;
            OnPropertyChanged();
        }
    }

    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }

    public SchoolSubjectItemViewModel(SchoolSubjectDto schoolSubject)
    {
        SchoolSubject = schoolSubject;
        EditCommand = new RelayCommand(() => RequestEdit?.Invoke(this));
        DeleteCommand = new RelayCommand(() => RequestDelete?.Invoke(this));
    }
}