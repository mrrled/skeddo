using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DboExtensions;

public static class DboTeacherExtensions
{
    public static Teacher ToTeacher(this DboTeacher dboTeacher, IMapper mapper)
    {
        return mapper.Map<Teacher>(dboTeacher);
    }
    
    public static Teacher ToTeacher(this DboTeacher dboTeacher, IMapper mapper,
        Action<IMappingOperationOptions<object, Teacher>> configure)
    {
        return mapper.Map(dboTeacher, configure);
    }

    public static List<Teacher> ToTeacher(this List<DboTeacher> dboTeachers, IMapper mapper)
    {
        return mapper.Map<List<Teacher>>(dboTeachers);
    }
    
    public static List<Teacher> ToTeacher(this List<DboTeacher> dboTeachers, IMapper mapper,
        Action<IMappingOperationOptions<object, List<Teacher>>> configure)
    {
        return mapper.Map(dboTeachers, configure);
    }
}