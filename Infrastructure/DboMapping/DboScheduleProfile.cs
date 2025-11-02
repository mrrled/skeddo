using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboScheduleProfile : Profile
{
    public DboScheduleProfile()
    {
        CreateMap<DboSchedule, Schedule>();
    }
}