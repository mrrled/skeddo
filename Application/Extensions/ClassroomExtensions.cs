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
    
    public static DtoClassroom ToClassroomDto(this Classroom classroom, IMapper mapper,
        Action<IMappingOperationOptions<object, DtoClassroom>> configure)
    {
        return mapper.Map(classroom, configure);
    }
    
    public static List<DtoClassroom> ToClassroomDto(this List<Classroom> classrooms, IMapper mapper)
    {
        return mapper.Map<List<DtoClassroom>>(classrooms);
    }
    
    public static List<DtoClassroom> ToClassroomDto(this List<Classroom> classrooms, IMapper mapper,
        Action<IMappingOperationOptions<object, List<DtoClassroom>>> configure)
    {
        return mapper.Map(classrooms, configure);
    }
}