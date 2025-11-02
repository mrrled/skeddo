using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboScheduleExtensions
{
    public static Schedule ToSchedule(this DboSchedule dboSchedule, IMapper mapper)
    {
        return mapper.Map<Schedule>(dboSchedule);
    }
    
    public static Schedule ToSchedule(this DboSchedule dboSchedule, IMapper mapper,
        Action<IMappingOperationOptions<object, Schedule>> configure)
    {
        return mapper.Map(dboSchedule, configure);
    }
    
    public static List<Schedule> ToSchedule(this List<DboSchedule> dboSchedules, IMapper mapper)
    {
        return mapper.Map<List<Schedule>>(dboSchedules);
    }
    
    public static List<Schedule> ToSchedule(this List<DboSchedule> dboSchedules, IMapper mapper,
        Action<IMappingOperationOptions<object, List<Schedule>>> configure)
    {
        return mapper.Map(dboSchedules, configure);
    }
}