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
}