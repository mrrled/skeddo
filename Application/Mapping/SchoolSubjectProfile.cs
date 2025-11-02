using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class SchoolSubjectProfile : Profile
{
    public SchoolSubjectProfile()
    {
        CreateMap<SchoolSubject, DtoSchoolSubject>();
    }
}