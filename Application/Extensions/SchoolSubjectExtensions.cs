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
    
    public static List<DtoSchoolSubject> ToSchoolSubjectDto(this ICollection<SchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<DtoSchoolSubject>>(schoolSubjects);
    }
    
    public static SchoolSubject ToSchoolSubject(this DtoSchoolSubject schoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubject>(schoolSubject);
    }
    
    public static List<SchoolSubject> ToSchoolSubject(this ICollection<DtoSchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubject>>(schoolSubjects);
    }
}