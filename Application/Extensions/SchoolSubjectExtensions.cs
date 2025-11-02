using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class SchoolSubjectExtensions
{
    public static DtoSchoolSubject ToSchoolSubjectDto(this SchoolSubject schoolSubject, IMapper mapper)
    {
        return mapper.Map<DtoSchoolSubject>(schoolSubject);
    }
    
    public static DtoSchoolSubject ToSchoolSubjectDto(this SchoolSubject schoolSubject, IMapper mapper,
        Action<IMappingOperationOptions<object, DtoSchoolSubject>> configure)
    {
        return mapper.Map(schoolSubject, configure);
    }
    
    public static List<DtoSchoolSubject> ToSchoolSubjectDto(this List<SchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<DtoSchoolSubject>>(schoolSubjects);
    }
    
    public static List<DtoSchoolSubject> ToSchoolSubjectDto(this List<SchoolSubject> schoolSubjects, IMapper mapper,
        Action<IMappingOperationOptions<object, List<DtoSchoolSubject>>> configure)
    {
        return mapper.Map(schoolSubjects, configure);
    }
}