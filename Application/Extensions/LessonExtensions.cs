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
    
    public static List<DtoLesson> ToLessonDto(this ICollection<Lesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<DtoLesson>>(lessons);
    }
    
    public static Lesson ToLesson(this DtoLesson lesson, IMapper mapper)
    {
        return mapper.Map<Lesson>(lesson);
    }
    
    public static List<Lesson> ToLesson(this ICollection<DtoLesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<Lesson>>(lessons);
    }
}