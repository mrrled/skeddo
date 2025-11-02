using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class StudyGroupExtensions
{
    public static DtoStudyGroup ToStudyGroupDto(this StudyGroup studyGroup, IMapper mapper)
    {
        return mapper.Map<DtoStudyGroup>(studyGroup);
    }
    
    public static List<DtoStudyGroup> ToStudyGroupDto(this List<StudyGroup> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<DtoStudyGroup>>(studyGroups);
    }
}