using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

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
    
    public static TeacherDbo ToTeacherDbo(this Teacher teacher, IMapper mapper)
    {
        return mapper.Map<TeacherDbo>(teacher);
    }
    
    public static List<TeacherDbo> ToTeacherDbo(this ICollection<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<TeacherDbo>>(teachers);
    }
}