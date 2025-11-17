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
    
    public static List<DtoTeacher> ToTeacherDto(this ICollection<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<DtoTeacher>>(teachers);
    }
    
    public static Teacher ToTeacher(this DtoTeacher teacher, IMapper mapper)
    {
        return mapper.Map<Teacher>(teacher);
    }
    
    public static List<Teacher> ToTeacher(this ICollection<DtoTeacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<Teacher>>(teachers);
    }
}