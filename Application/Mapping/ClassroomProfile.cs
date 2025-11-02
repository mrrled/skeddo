using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class ClassroomProfile : Profile
{
    public ClassroomProfile()
    {
        CreateMap<Classroom, DtoClassroom>();
    }
}