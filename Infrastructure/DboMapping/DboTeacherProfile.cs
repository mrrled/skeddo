using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboTeacherProfile : Profile
{
    public DboTeacherProfile()
    {
        CreateMap<DboTeacher, Teacher>();
    }
}