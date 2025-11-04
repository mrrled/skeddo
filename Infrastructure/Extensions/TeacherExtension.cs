using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class TeacherExtension
{
    public static TeacherDbo ToTeacherDbo(this Teacher teacher, IMapper mapper)
    {
        return mapper.Map<TeacherDbo>(teacher);
    }
    
    public static List<TeacherDbo> ToTeacherDbo(this ICollection<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<TeacherDbo>>(teachers);
    }
}