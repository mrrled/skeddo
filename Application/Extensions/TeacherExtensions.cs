using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class TeacherExtensions
{
    public static DtoTeacher ToTeacherDto(this Teacher teacher, IMapper mapper)
    {
        return mapper.Map<DtoTeacher>(teacher);
    }
    
    public static DtoTeacher ToTeacherDto(this Teacher teacher, IMapper mapper,
        Action<IMappingOperationOptions<object, DtoTeacher>> configure)
    {
        return mapper.Map(teacher, configure);
    }
    
    public static List<DtoTeacher> ToTeacherDto(this List<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<DtoTeacher>>(teachers);
    }
    
    public static List<DtoTeacher> ToTeacherDto(this List<Teacher> teachers, IMapper mapper,
        Action<IMappingOperationOptions<object, List<DtoTeacher>>> configure)
    {
        return mapper.Map(teachers, configure);
    }
}