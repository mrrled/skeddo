using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class ScheduleDtoExtensions
{
    public static ScheduleDto ToScheduleDto(this Schedule schedule)
    {
        return DtoMapper.ToScheduleDto(schedule);
    }
    
    public static List<ScheduleDto> ToSchedulesDto(this ICollection<Schedule> schedules)
    {
        return DtoMapper.ToScheduleDto(schedules);
    }
    
    public static Schedule ToSchedule(this ScheduleDto schedule)
    {
        return DtoMapper.ToSchedule(schedule);
    }
    
    public static List<Schedule> ToSchedules(this ICollection<ScheduleDto> schedules)
    {
        return DtoMapper.ToSchedule(schedules);
    }
}