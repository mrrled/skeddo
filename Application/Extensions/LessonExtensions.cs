using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class LessonExtensions
{
    public static DtoLesson ToLessonDto(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<DtoLesson>(lesson);
    }
    
    public static List<DtoLesson> ToLessonDto(this List<Lesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<DtoLesson>>(lessons);
    }
}