using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class ScheduleDboExtension
{
    public static Schedule ToSchedule(this ScheduleDbo schedule)
    {
        return DboMapper.ToSchedule(schedule);
    }
    
    public static List<Schedule> ToSchedules(this ICollection<ScheduleDbo> schedules)
    {
        return DboMapper.ToSchedule(schedules);
    }
    
    public static ScheduleDbo ToScheduleDbo(this Schedule schedule)
    {
        return DboMapper.ToScheduleDbo(schedule);
    }
    
    public static List<ScheduleDbo> ToSchedulesDbo(this ICollection<Schedule> schedules)
    {
        return DboMapper.ToScheduleDbo(schedules);
    }
}