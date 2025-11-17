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
    
    public static List<DtoSchedule> ToScheduleDto(this ICollection<Schedule> schedules, IMapper mapper)
    {
        return mapper.Map<List<DtoSchedule>>(schedules);
    }
    
    public static Schedule ToSchedule(this DtoSchedule schedule, IMapper mapper)
    {
        return mapper.Map<Schedule>(schedule);
    }
    
    public static List<Schedule> ToSchedule(this ICollection<DtoSchedule> schedules, IMapper mapper)
    {
        return mapper.Map<List<Schedule>>(schedules);
    }
}