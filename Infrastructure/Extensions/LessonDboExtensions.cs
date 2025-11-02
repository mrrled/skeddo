using AutoMapper;
using Domain.Models;
using Infrastructure.Entities;

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
}