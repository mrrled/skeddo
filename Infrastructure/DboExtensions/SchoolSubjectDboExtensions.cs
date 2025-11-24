using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class SchoolSubjectDboExtensions
{
    public static SchoolSubject ToSchoolSubject(this SchoolSubjectDbo schoolSubject)
    {
        return DboMapper.ToSchoolSubject(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubjects(this ICollection<SchoolSubjectDbo> schoolSubjects)
    {
        return DboMapper.ToSchoolSubject(schoolSubjects);
    }
    
    public static SchoolSubjectDbo ToSchoolSubjectDbo(this SchoolSubject schoolSubject)
    {
        return DboMapper.ToSchoolSubjectDbo(schoolSubject);
    }
    
    public static List<SchoolSubjectDbo> ToSchoolSubjectsDbo(this ICollection<SchoolSubject> schoolSubjects)
    {
        return DboMapper.ToSchoolSubjectDbo(schoolSubjects);
    }
}