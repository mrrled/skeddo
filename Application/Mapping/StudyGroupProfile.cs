using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class StudyGroupProfile : Profile
{
    public StudyGroupProfile()
    {
        CreateMap<StudyGroup, DtoStudyGroup>();
    }
}