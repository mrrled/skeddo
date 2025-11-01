using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class TeacherExtensions
{
    public static TeacherDto ToTeacherDto(this Teacher teacher, IMapper mapper)
    {
        return mapper.Map<TeacherDto>(teacher);
    }
    
    public static List<TeacherDto> ToTeacherDto(this List<Teacher> teachers, IMapper mapper)
    {
        return mapper.Map<List<TeacherDto>>(teachers);
    }
}