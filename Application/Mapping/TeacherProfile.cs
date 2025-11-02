using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, DtoTeacher>()
            .ForMember(dest => dest.Specialty,
                opt => opt.MapFrom(src => src.Specializations.FirstOrDefault()!.Name));
    }
}