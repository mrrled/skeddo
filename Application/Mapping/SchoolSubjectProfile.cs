using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class SchoolSubjectProfile : Profile
{
    public SchoolSubjectProfile()
    {
        CreateMap<SchoolSubject, SchoolSubjectDto>();
    }
}