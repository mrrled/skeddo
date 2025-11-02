using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboSchoolSubjectProfile : Profile
{
    public DboSchoolSubjectProfile()
    {
        CreateMap<DboSchoolSubject, SchoolSubject>();
    }
}