using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class TeacherDboExtensions
{
    public static Teacher ToTeacher(this TeacherDbo teacher)
    {
        return DboMapper.ToTeacher(teacher);
    }
    
    public static List<Teacher> ToTeacher(this ICollection<TeacherDbo> teachers)
    {
        return DboMapper.ToTeacher(teachers);
    }
    
    public static TeacherDbo ToTeacherDbo(this Teacher teacher)
    {
        return DboMapper.ToTeacherDbo(teacher);
    }
    
    public static List<TeacherDbo> ToTeacherDbo(this ICollection<Teacher> teachers)
    {
        return DboMapper.ToTeacherDbo(teachers);
    }
}