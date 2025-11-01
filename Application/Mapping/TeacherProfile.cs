using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, TeacherDto>()
            .ForMember(dest => dest.Specialty,
                opt => opt.MapFrom(src => src.Specializations.FirstOrDefault()!.Name));
    }
}