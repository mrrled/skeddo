using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class StudyGroupDboExtensions
{
    public static StudyGroup ToStudyGroup(this StudyGroupDbo studyGroup)
    {
        return DboMapper.ToStudyGroup(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(this ICollection<StudyGroupDbo> studyGroups)
    {
        return DboMapper.ToStudyGroup(studyGroups);
    }
    
    public static StudyGroupDbo ToStudyGroupDbo(this StudyGroup studyGroup)
    {
        return DboMapper.ToStudyGroupDbo(studyGroup);
    }
    
    public static List<StudyGroupDbo> ToStudyGroupDbo(this ICollection<StudyGroup> studyGroups)
    {
        return DboMapper.ToStudyGroupDbo(studyGroups);
    }
}