using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class StudyGroupDboExtensions
{
    public static StudyGroup ToStudyGroup(this StudyGroupDbo studyGroup)
    {
        return DboMapper.ToStudyGroup(studyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroups(this ICollection<StudyGroupDbo> studyGroups)
    {
        return DboMapper.ToStudyGroup(studyGroups);
    }
    
    public static StudyGroupDbo ToStudyGroupDbo(this StudyGroup studyGroup)
    {
        return DboMapper.ToStudyGroupDbo(studyGroup);
    }
    
    public static List<StudyGroupDbo> ToStudyGroupsDbo(this ICollection<StudyGroup> studyGroups)
    {
        return DboMapper.ToStudyGroupDbo(studyGroups);
    }
}