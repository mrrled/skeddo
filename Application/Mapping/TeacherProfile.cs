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
                opt => opt.MapFrom(src => src.SchoolSubjects.FirstOrDefault()!.Name))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.FullName.FirstName))
            .ForMember(dest => dest.Surname,
                opt => opt.MapFrom(src => src.FullName.LastName))
            .ForMember(dest => dest.Patronymic,
                opt => opt.MapFrom(src => src.FullName.Patronymic));
    }
}