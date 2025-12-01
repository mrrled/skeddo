using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjects;

public class SchoolSubjectCreationViewModel : ViewModelBase
{
    private SchoolSubjectDto schoolSubject = new();
    private ISchoolSubjectServices service;

    public SchoolSubjectDto SchoolSubject
    {
        get => schoolSubject;
        set => SetProperty(ref schoolSubject, value);
    } 

    public SchoolSubjectCreationViewModel(ISchoolSubjectServices service)
    {
        this.service = service;
        SaveChangesCommand = new RelayCommandAsync(SaveChanges);
    }
    
    public ICommand SaveChangesCommand { get; set; }

    public Task SaveChanges()
    {
        service.AddSchoolSubject(schoolSubject);
        return Task.CompletedTask;
    }
    public Task SetName(string name)
    {
        schoolSubject.Name = name;
        return Task.CompletedTask;
    }
}