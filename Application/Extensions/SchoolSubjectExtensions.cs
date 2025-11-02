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
    
    public static List<DtoSchoolSubject> ToSchoolSubjectDto(this List<SchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<DtoSchoolSubject>>(schoolSubjects);
    }
}