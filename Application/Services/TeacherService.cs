using Application.Converters;
using Application.UIModels;
using Domain;

namespace Application.Services;

public class TeacherService : ITeacherService 
{
    public List<Teacher> FetchTeachersFromBackend()
    {
        return repository.GetTeachers().Select(TeacherConverter.Convert).ToList();
    }

    private IScheduleRepository repository;

    public TeacherService(IScheduleRepository repository)
    {
        this.repository = repository;
    }
}