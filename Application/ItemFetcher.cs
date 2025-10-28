using Application.Converters;
using Application.UIModels;
using Infrastructure.Repositories;

namespace Application;

public static class ItemFetcher
{
    public static List<Teacher> FetchTeachersFromBackend()
    {
        return TeacherRepository.GetTeachers().Select(TeacherConverter.Convert).ToList();
    }
}