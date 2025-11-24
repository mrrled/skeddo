using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class ClassroomDtoExtensions
{
    public static ClassroomDto ToClassroomDto(this Classroom classroom)
    {
        return DtoMapper.ToClassroomDto(classroom);
    }
    
    public static List<ClassroomDto> ToClassroomsDto(this ICollection<Classroom> classrooms)
    {
        return DtoMapper.ToClassroomDto(classrooms);
    }
    
    public static Classroom ToClassroom(this ClassroomDto classroom)
    {
        return DtoMapper.ToClassroom(classroom);
    }
    
    public static List<Classroom> ToClassrooms(this ICollection<ClassroomDto> classrooms)
    {
        return DtoMapper.ToClassroom(classrooms);
    }
    
}