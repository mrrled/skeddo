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
    
    public static List<DtoStudyGroup> ToStudyGroupDto(this ICollection<StudyGroup> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<DtoStudyGroup>>(studyGroups);
    }
    public static StudyGroup ToStudyGroup(this DtoStudyGroup studyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroup>(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(this ICollection<DtoStudyGroup> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<StudyGroup>>(studyGroups);
    }
}