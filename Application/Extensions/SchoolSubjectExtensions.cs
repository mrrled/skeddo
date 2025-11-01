using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class SchoolSubjectExtensions
{
    public static SchoolSubjectDto ToSchoolSubjectDto(this SchoolSubject schoolSubject, IMapper mapper)
    {
        return mapper.Map<SchoolSubjectDto>(schoolSubject);
    }
    
    public static List<SchoolSubjectDto> ToSchoolSubjectDto(this List<SchoolSubject> schoolSubjects, IMapper mapper)
    {
        return mapper.Map<List<SchoolSubjectDto>>(schoolSubjects);
    }
}