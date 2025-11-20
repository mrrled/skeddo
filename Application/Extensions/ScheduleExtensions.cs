using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ScheduleExtensions
{
    public static ScheduleDto ToScheduleDto(this Schedule schedule)
    {
        return DtoMapper.ToScheduleDto(schedule);
    }
    
    public static List<ScheduleDto> ToScheduleDto(this ICollection<Schedule> schedules)
    {
        return DtoMapper.ToScheduleDto(schedules);
    }
    
    public static Schedule ToSchedule(this ScheduleDto schedule)
    {
        return DtoMapper.ToSchedule(schedule);
    }
    
    public static List<Schedule> ToSchedule(this ICollection<ScheduleDto> schedules)
    {
        return DtoMapper.ToSchedule(schedules);
    }
}