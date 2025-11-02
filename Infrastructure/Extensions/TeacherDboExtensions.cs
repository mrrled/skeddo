using AutoMapper;
using Domain.Models;
using Infrastructure.Entities;

namespace Infrastructure.Extensions;

public static class TeacherDboExtensions
{
    public static Teacher ToTeacher(this TeacherDbo teacher, IMapper mapper)
    {
        return mapper.Map<Teacher>(teacher);
    }
    
    public static List<Teacher> ToTeacher(this ICollection<TeacherDbo> teachers, IMapper mapper)
    {
        return mapper.Map<List<Teacher>>(teachers);
    }
}