using Application.UIModels;
using Domain;

namespace Application.Services;

public interface ITeacherService
{
    public List<Teacher> FetchTeachersFromBackend();
}