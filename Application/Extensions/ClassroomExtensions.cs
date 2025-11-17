using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ClassroomExtensions
{
    public static DtoClassroom ToClassroomDto(this Classroom classroom, IMapper mapper)
    {
        return mapper.Map<DtoClassroom>(classroom);
    }
    
    public static List<DtoClassroom> ToClassroomDto(this ICollection<Classroom> classrooms, IMapper mapper)
    {
        return mapper.Map<List<DtoClassroom>>(classrooms);
    }
    
    public static Classroom ToClassroom(this DtoClassroom classroom, IMapper mapper)
    {
        return mapper.Map<Classroom>(classroom);
    }
    
    public static List<Classroom> ToClassroom(this ICollection<DtoClassroom> classrooms, IMapper mapper)
    {
        return mapper.Map<List<Classroom>>(classrooms);
    }
    
}