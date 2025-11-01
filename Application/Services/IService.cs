using Application.UIModels;
using Domain;
using Domain.Models;

namespace Application.Services;

public interface IService
{
    public List<TeacherDto> FetchTeachersFromBackend();
}