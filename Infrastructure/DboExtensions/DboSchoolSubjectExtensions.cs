using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboSchoolSubjectExtensions
{
    public static SchoolSubject ToSchoolSubjectDto(this DboSchoolSubject dboSchoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubject>(dboSchoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubjectDto(this List<DboSchoolSubject> dboSchoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubject>>(dboSchoolSubjects);
    }
}