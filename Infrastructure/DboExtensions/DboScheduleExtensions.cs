using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboScheduleExtensions
{
    public static Schedule ToScheduleDto(this DboSchedule dboSchedule, IMapper mapper)
    {
        return mapper.Map<Schedule>(dboSchedule);
    }
    
    public static List<Schedule> ToScheduleDto(this List<DboSchedule> dboSchedules, IMapper mapper)
    {
        return mapper.Map<List<Schedule>>(dboSchedules);
    }
}