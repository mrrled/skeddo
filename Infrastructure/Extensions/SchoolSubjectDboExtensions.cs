using AutoMapper;
using Domain.Models;
using Infrastructure.Entities;

namespace Infrastructure.Extensions;

public static class SchoolSubjectDboExtensions
{
    public static SchoolSubject ToSchoolSubject(this SchoolSubjectDbo schoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubject>(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(this ICollection<SchoolSubjectDbo> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubject>>(schoolSubjects);
    }
}