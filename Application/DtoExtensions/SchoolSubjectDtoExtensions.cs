using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class SchoolSubjectDtoExtensions
{
    public static SchoolSubjectDto ToSchoolSubjectDto(this SchoolSubject schoolSubject)
    {
        return DtoMapper.ToSchoolSubjectDto(schoolSubject);
    }
    
    public static List<SchoolSubjectDto> ToSchoolSubjectsDto(this ICollection<SchoolSubject> schoolSubjects)
    {
        return DtoMapper.ToSchoolSubjectDto(schoolSubjects);
    }
    
    public static SchoolSubject ToSchoolSubject(this SchoolSubjectDto schoolSubject)
    {
        return DtoMapper.ToSchoolSubject(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubjects(this ICollection<SchoolSubjectDto> schoolSubjects)
    {
        return DtoMapper.ToSchoolSubject(schoolSubjects);
    }
}