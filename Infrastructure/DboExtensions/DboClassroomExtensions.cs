using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboClassroomExtensions
{
    public static Classroom ToClassroom(this DboClassroom dboClassroom, IMapper mapper)
    {
        return mapper.Map<Classroom>(dboClassroom);
    }
    
    public static List<Classroom> ToClassroom(this List<DboClassroom> dboClassrooms, IMapper mapper)
    {
        return mapper.Map<List<Classroom>>(dboClassrooms);
    }
}