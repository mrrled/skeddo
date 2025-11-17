using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class StudyGroupDboExtensions
{
    public static StudyGroup ToStudyGroup(this StudyGroupDbo studyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroup>(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(this ICollection<StudyGroupDbo> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<StudyGroup>>(studyGroups);
    }
    
    public static StudyGroupDbo ToStudyGroupDbo(this StudyGroup studyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroupDbo>(studyGroup);
    }
    
    public static List<StudyGroupDbo> ToStudyGroupDbo(this ICollection<StudyGroup> studyGroups, IMapper mapper)
    {
        return mapper.Map<List<StudyGroupDbo>>(studyGroups);
    }
}