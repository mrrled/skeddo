using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class ClassroomDboExtensions
{
    public static Classroom ToClassroom(this ClassroomDbo classroom)
    {
        return DboMapper.ToClassroom(classroom);
    }
    
    public static List<Classroom> ToClassrooms(this ICollection<ClassroomDbo> classrooms)
    {
        return DboMapper.ToClassroom(classrooms);
    }
    
    public static ClassroomDbo ToClassroomDbo(this Classroom classroom)
    {
        return DboMapper.ToClassroomDbo(classroom);
    }
    
    public static List<ClassroomDbo> ToClassroomsDbo(this ICollection<Classroom> classrooms)
    {
        return DboMapper.ToClassroomDbo(classrooms);
    }
}