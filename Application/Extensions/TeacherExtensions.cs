using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class TeacherExtensions
{
    public static TeacherDto ToTeacherDto(this Teacher teacher)
    {
        return DtoMapper.ToTeacherDto(teacher);
    }
    
    public static List<TeacherDto> ToTeacherDto(this ICollection<Teacher> teachers)
    {
        return DtoMapper.ToTeacherDto(teachers);
    }
    
    public static Teacher ToTeacher(this TeacherDto teacher)
    {
        return DtoMapper.ToTeacher(teacher);
    }
    
    public static List<Teacher> ToTeacher(this ICollection<TeacherDto> teachers)
    {
        return DtoMapper.ToTeacher(teachers);
    }
}