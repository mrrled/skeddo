using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class ScheduleDboExtension
{
    public static Schedule ToSchedule(this ScheduleDbo schedule)
    {
        return DboMapper.ToSchedule(schedule);
    }
    
    public static List<Schedule> ToSchedule(this ICollection<ScheduleDbo> schedules)
    {
        return DboMapper.ToSchedule(schedules);
    }
    
    public static ScheduleDbo ToScheduleDbo(this Schedule schedule)
    {
        return DboMapper.ToScheduleDbo(schedule);
    }
    
    public static List<ScheduleDbo> ToScheduleDbo(this ICollection<Schedule> schedules)
    {
        return DboMapper.ToScheduleDbo(schedules);
    }
}