using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ScheduleExtensions
{
    public static ScheduleDto ToScheduleDto(this Schedule schedule, IMapper mapper)
    {
        return mapper.Map<ScheduleDto>(schedule);
    }
    
    public static List<ScheduleDto> ToScheduleDto(this List<Schedule> schedules, IMapper mapper)
    {
        return mapper.Map<List<ScheduleDto>>(schedules);
    }
}