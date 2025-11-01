using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class StudyGroupProfile : Profile
{
    public StudyGroupProfile()
    {
        CreateMap<StudyGroup, StudyGroupDto>();
    }
}