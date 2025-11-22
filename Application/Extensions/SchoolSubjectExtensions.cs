using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class SchoolSubjectExtensions
{
    public static SchoolSubjectDto ToSchoolSubjectDto(this SchoolSubject schoolSubject)
    {
        return DtoMapper.ToSchoolSubjectDto(schoolSubject);
    }
    
    public static List<SchoolSubjectDto> ToSchoolSubjectDto(this ICollection<SchoolSubject> schoolSubjects)
    {
        return DtoMapper.ToSchoolSubjectDto(schoolSubjects);
    }
    
    public static SchoolSubject ToSchoolSubject(this SchoolSubjectDto schoolSubject)
    {
        return DtoMapper.ToSchoolSubject(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(this ICollection<SchoolSubjectDto> schoolSubjects)
    {
        return DtoMapper.ToSchoolSubject(schoolSubjects);
    }
}