using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ClassroomExtensions
{
    public static ClassroomDto ToClassroomDto(this Classroom classroom, IMapper mapper)
    {
        return mapper.Map<ClassroomDto>(classroom);
    }
    
    public static List<ClassroomDto> ToClassroomDto(this List<Classroom> classrooms, IMapper mapper)
    {
        return mapper.Map<List<ClassroomDto>>(classrooms);
    }
}