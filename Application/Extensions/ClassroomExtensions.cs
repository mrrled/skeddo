using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ClassroomExtensions
{
    public static ClassroomDto ToClassroomDto(this Classroom classroom)
    {
        return DtoMapper.ToClassroomDto(classroom);
    }
    
    public static List<ClassroomDto> ToClassroomDto(this ICollection<Classroom> classrooms)
    {
        return DtoMapper.ToClassroomDto(classrooms);
    }
    
    public static Classroom ToClassroom(this ClassroomDto classroom)
    {
        return DtoMapper.ToClassroom(classroom);
    }
    
    public static List<Classroom> ToClassroom(this ICollection<ClassroomDto> classrooms)
    {
        return DtoMapper.ToClassroom(classrooms);
    }
    
}