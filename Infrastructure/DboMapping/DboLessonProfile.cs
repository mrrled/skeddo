using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboMapping;

public class DboLessonProfile : Profile
{
    public DboLessonProfile()
    {
        CreateMap<DboLesson, Lesson>();
    }
}