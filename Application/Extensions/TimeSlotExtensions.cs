using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class TimeSlotExtensions
{
    public static TimeSlotDto ToTimeSlotDto(this TimeSlot timeSlot, IMapper mapper)
    {
        return mapper.Map<TimeSlotDto>(timeSlot);
    }
    
    public static List<TimeSlotDto> ToTimeSlotDto(this List<TimeSlot> timeSlots, IMapper mapper)
    {
        return mapper.Map<List<TimeSlotDto>>(timeSlots);
    }
}