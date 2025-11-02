using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboStudyGroupExtensions
{
    public static StudyGroup ToStudyGroupDto(this DboStudyGroup dboStudyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroup>(dboStudyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroupDto(this List<DboStudyGroup> dboStudyGroup, IMapper mapper)
    {
        return mapper.Map<List<StudyGroup>>(dboStudyGroup);
    }
}