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
    
    public static List<DtoClassroom> ToClassroomDto(this List<Classroom> classrooms, IMapper mapper)
    {
        return mapper.Map<List<DtoClassroom>>(classrooms);
    }
}