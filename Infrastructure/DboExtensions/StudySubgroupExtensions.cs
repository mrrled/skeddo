using Domain.Models;
using Infrastructure.DboMapping;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class StudySubgroupExtensions
{
    public static StudySubgroup ToStudySubgroup(this StudySubgroupDbo lesson)
    {
        return DboMapper.ToStudySubgroup(lesson);
    }
    
    public static List<StudySubgroup> ToStudySubgroups(this ICollection<StudySubgroupDbo> lessons)
    {
        return DboMapper.ToStudySubgroup(lessons);
    }
    
    public static StudySubgroupDbo ToStudySubgroupDbo(this StudySubgroup lesson)
    {
        return DboMapper.ToStudySubgroupDbo(lesson);
    }
    
    public static List<StudySubgroupDbo> ToStudySubgroupsDbo(this ICollection<StudySubgroup> lessons)
    {
        return DboMapper.ToStudySubgroupDbo(lessons);
    }
}