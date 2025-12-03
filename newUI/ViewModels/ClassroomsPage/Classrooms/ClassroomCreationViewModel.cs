using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;

namespace newUI.ViewModels.ClassroomsPage.Classrooms;

public class ClassroomCreationViewModel : ViewModelBase
{
    private ClassroomDto classroom = new();
    private readonly IServiceScopeFactory scopeFactory;

    public ClassroomDto Classroom
    {
        get => classroom;
        set => SetProperty(ref classroom, value);
    } 

    public ClassroomCreationViewModel(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        SaveChangesCommand = new RelayCommandAsync(SaveChanges);
    }
    
    public ICommand SaveChangesCommand { get; set; }

    public Task SaveChanges()
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IClassroomServices>();
            service.AddClassroom(classroom);
        }

        return Task.CompletedTask;
    }
    
    public Task SetName(string name)
    {
        classroom.Name = name;
        return Task.CompletedTask;
    }
}