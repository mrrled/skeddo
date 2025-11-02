using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class ClassroomDboExtensions
{
    public static Classroom ToClassroom(this ClassroomDbo classroom, IMapper mapper)
    {
        return mapper.Map<Classroom>(classroom);
    }
    
    public static List<Classroom> ToClassroom(this ICollection<ClassroomDbo> classrooms, IMapper mapper)
    {
        return mapper.Map<List<Classroom>>(classrooms);
    }
}