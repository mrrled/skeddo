using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<Schedule, DtoSchedule>();
    }
}