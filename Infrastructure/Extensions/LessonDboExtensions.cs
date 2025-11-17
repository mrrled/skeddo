using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class LessonDboExtensions
{
    public static Lesson ToLesson(this LessonDbo lesson, IMapper mapper)
    {
        return mapper.Map<Lesson>(lesson);
    }
    
    public static List<Lesson> ToLesson(this ICollection<LessonDbo> lessons, IMapper mapper)
    {
        return mapper.Map<List<Lesson>>(lessons);
    }
    
    public static LessonDbo ToLesson(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<LessonDbo>(lesson);
    }
    
    public static List<LessonDbo> ToLesson(this ICollection<Lesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<LessonDbo>>(lessons);
    }
}