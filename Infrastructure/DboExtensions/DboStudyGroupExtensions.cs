using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboStudyGroupExtensions
{
    public static StudyGroup ToStudyGroup(this DboStudyGroup dboStudyGroup, IMapper mapper)
    {
        return mapper.Map<StudyGroup>(dboStudyGroup);
    }
    
    public static StudyGroup ToStudyGroup(this DboStudyGroup dboStudyGroup, IMapper mapper,
        Action<IMappingOperationOptions<object, StudyGroup>> configure)
    {
        return mapper.Map(dboStudyGroup, configure);
    }
    
    public static List<StudyGroup> ToStudyGroup(this List<DboStudyGroup> dboStudyGroup, IMapper mapper)
    {
        return mapper.Map<List<StudyGroup>>(dboStudyGroup);
    }
    
    public static List<StudyGroup> ToStudyGroup(this List<DboStudyGroup> dboStudyGroup, IMapper mapper,
        Action<IMappingOperationOptions<object, List<StudyGroup>>> configure)
    {
        return mapper.Map(dboStudyGroup, configure);
    }
}