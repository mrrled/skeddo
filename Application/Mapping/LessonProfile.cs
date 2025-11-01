using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mapping;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<Lesson, LessonDto>();
    }
}