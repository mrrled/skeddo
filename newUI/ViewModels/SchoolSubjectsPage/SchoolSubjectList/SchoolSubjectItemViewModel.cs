using Application.DtoModels;
using Application.IServices;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectList;

public class SchoolSubjectItemViewModel : ViewModelBase
{
    private SchoolSubjectDto schoolSubject;
    private ISchoolSubjectServices service;

    public SchoolSubjectDto SchoolSubject
    {
        get => schoolSubject;
        set => SetProperty(ref schoolSubject, value);
    }
}