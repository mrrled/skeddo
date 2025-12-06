using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class TeacherDtoExtensions
{
    public static TeacherDto ToTeacherDto(this Teacher teacher)
    {
        return DtoMapper.ToTeacherDto(teacher);
    }
    
    public static List<TeacherDto> ToTeachersDto(this ICollection<Teacher> teachers)
    {
        return DtoMapper.ToTeacherDto(teachers);
    }
}