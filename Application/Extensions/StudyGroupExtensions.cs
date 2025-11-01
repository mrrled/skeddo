using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class StudyGroupExtensions
{
    public static StudyGroupDto ToStudyGroupDto(this StudyGroup studyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroupDto>(studyGroup);
    }
    
    public static List<StudyGroupDto> ToStudyGroupDto(this List<StudyGroup> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<StudyGroupDto>>(studyGroups);
    }
}