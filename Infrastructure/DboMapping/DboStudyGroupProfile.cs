using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboStudyGroupProfile : Profile
{
    public DboStudyGroupProfile()
    {
        CreateMap<DboStudyGroup, StudyGroup>();
    }
}