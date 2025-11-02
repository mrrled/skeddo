using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboSchoolSubjectExtensions
{
    public static SchoolSubject ToSchoolSubject(this DboSchoolSubject dboSchoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubject>(dboSchoolSubject);
    }
    
    public static SchoolSubject ToSchoolSubject(this DboSchoolSubject dboSchoolSubject, IMapper mapper,
        Action<IMappingOperationOptions<object, SchoolSubject>> configure)
    {
        return mapper.Map(dboSchoolSubject, configure);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(this List<DboSchoolSubject> dboSchoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubject>>(dboSchoolSubjects);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(this List<DboSchoolSubject> dboSchoolSubjects, IMapper mapper,
        Action<IMappingOperationOptions<object, List<SchoolSubject>>> configure)
    {
        return mapper.Map(dboSchoolSubjects, configure);
    }
}