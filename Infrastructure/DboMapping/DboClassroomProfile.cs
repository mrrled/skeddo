using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboClassroomProfile : Profile
{
    public DboClassroomProfile()
    {
        CreateMap<DboClassroom, Classroom>();
    }
}