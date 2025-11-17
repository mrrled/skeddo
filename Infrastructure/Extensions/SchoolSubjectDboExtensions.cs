using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

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
    
    public static SchoolSubjectDbo ToSchoolSubjectDbo(this SchoolSubject schoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubjectDbo>(schoolSubject);
    }
    
    public static List<SchoolSubjectDbo> ToSchoolSubjectDbo(this ICollection<SchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubjectDbo>>(schoolSubjects);
    }
}