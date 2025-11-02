using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class ScheduleExtensions
{
    public static DtoSchedule ToScheduleDto(this Schedule schedule, IMapper mapper)
    {
        return mapper.Map<DtoSchedule>(schedule);
    }
    
    public static List<DtoSchedule> ToScheduleDto(this List<Schedule> schedules, IMapper mapper)
    {
        return mapper.Map<List<DtoSchedule>>(schedules);
    }
}