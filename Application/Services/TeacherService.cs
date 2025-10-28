using Application.Converters;
using Application.UIModels;
using Domain;
using Infrastructure.Repositories;

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