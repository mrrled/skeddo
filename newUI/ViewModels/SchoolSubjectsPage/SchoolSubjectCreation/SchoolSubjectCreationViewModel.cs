using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.SchoolSubjectsPage.SchoolSubjectCreation;

public class SchoolSubjectCreationViewModel : ViewModelBase
{
    private SchoolSubjectDto schoolSubject = new();
    private readonly IServiceScopeFactory scopeFactory;

    public SchoolSubjectDto SchoolSubject
    {
        get => schoolSubject;
        set => SetProperty(ref schoolSubject, value);
    } 

    public SchoolSubjectCreationViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveChangesCommand = new RelayCommandAsync(SaveChanges);
    }
    
    public ICommand SaveChangesCommand { get; set; }

    public Task SaveChanges()
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ISchoolSubjectServices>();
            service.AddSchoolSubject(schoolSubject);
        }

        return Task.CompletedTask;
    }
    
    public Task SetName(string name)
    {
        schoolSubject.Name = name;
        return Task.CompletedTask;
    }
}