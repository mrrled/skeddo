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
    
    public static List<DtoTeacher> ToTeacherDto(this List<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<DtoTeacher>>(teachers);
    }
}