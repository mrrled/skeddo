using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class TeacherDboExtensions
{
    public static Teacher ToTeacher(this TeacherDbo teacher)
    {
        return DboMapper.ToTeacher(teacher);
    }
    
    public static List<Teacher> ToTeachers(this ICollection<TeacherDbo> teachers)
    {
        return DboMapper.ToTeacher(teachers);
    }
    
    public static TeacherDbo ToTeacherDbo(this Teacher? teacher)
    {
        return DboMapper.ToTeacherDbo(teacher);
    }
    
    public static List<TeacherDbo> ToTeachersDbo(this ICollection<Teacher> teachers)
    {
        return DboMapper.ToTeacherDbo(teachers);
    }
}