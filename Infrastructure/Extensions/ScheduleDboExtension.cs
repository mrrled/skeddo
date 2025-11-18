using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class ScheduleDboExtension
{
    public static Schedule ToSchedule(this ScheduleDbo schedule, IMapper mapper)
    {
        return mapper.Map<Schedule>(schedule);
    }
    
    public static List<Schedule> ToSchedule(this ICollection<ScheduleDbo> schedules, IMapper mapper)
    {
        return mapper.Map<List<Schedule>>(schedules);
    }
    
    public static ScheduleDbo ToScheduleDbo(this Schedule schedule, IMapper mapper)
    {
        return mapper.Map<ScheduleDbo>(schedule);
    }
    
    public static List<ScheduleDbo> ToScheduleDbo(this ICollection<Schedule> schedules, IMapper mapper)
    {
        return mapper.Map<List<ScheduleDbo>>(schedules);
    }
}