using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class TimeSlotExtensions
{
    public static DtoTimeSlot ToTimeSlotDto(this TimeSlot timeSlot, IMapper mapper)
    {
        return mapper.Map<DtoTimeSlot>(timeSlot);
    }
    
    public static List<DtoTimeSlot> ToTimeSlotDto(this List<TimeSlot> timeSlots, IMapper mapper)
    {
        return mapper.Map<List<DtoTimeSlot>>(timeSlots);
    }
}