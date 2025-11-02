using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class TimeSlotProfile : Profile
{
    public TimeSlotProfile()
    {
        CreateMap<TimeSlot, DtoTimeSlot>();
    }
}