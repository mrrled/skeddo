using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboTeacherExtensions
{
    public static Teacher ToTeacherDto(this DboTeacher dboTeacher, IMapper mapper)
    {
        return mapper.Map<Teacher>(dboTeacher);
    }
    
    public static List<Teacher> ToTeacherDto(this List<DboTeacher> dboTeachers, IMapper mapper)
    {
        return mapper.Map<List<Teacher>>(dboTeachers);
    }
}