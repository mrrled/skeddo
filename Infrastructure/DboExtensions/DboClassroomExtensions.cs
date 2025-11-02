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
    
    public static Classroom ToClassroom(this DboClassroom dboClassroom, IMapper mapper,
        Action<IMappingOperationOptions<object, Classroom>> configure)
    {
        return mapper.Map(dboClassroom, configure);
    }
    
    public static List<Classroom> ToClassroom(this List<DboClassroom> dboClassrooms, IMapper mapper)
    {
        return mapper.Map<List<Classroom>>(dboClassrooms);
    }
    
    public static List<Classroom> ToClassroom(this List<DboClassroom> dboClassrooms, IMapper mapper,
        Action<IMappingOperationOptions<object, List<Classroom>>> configure)
    {
        return mapper.Map(dboClassrooms, configure);
    }
}