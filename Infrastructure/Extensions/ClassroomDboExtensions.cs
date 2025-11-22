using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class ClassroomDboExtensions
{
    public static Classroom ToClassroom(this ClassroomDbo classroom)
    {
        return DboMapper.ToClassroom(classroom);
    }
    
    public static List<Classroom> ToClassroom(this ICollection<ClassroomDbo> classrooms)
    {
        return DboMapper.ToClassroom(classrooms);
    }
    
    public static ClassroomDbo ToClassroomDbo(this Classroom classroom)
    {
        return DboMapper.ToClassroomDbo(classroom);
    }
    
    public static List<ClassroomDbo> ToClassroomDbo(this ICollection<Classroom> classrooms)
    {
        return DboMapper.ToClassroomDbo(classrooms);
    }
}