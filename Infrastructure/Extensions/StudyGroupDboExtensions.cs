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
}