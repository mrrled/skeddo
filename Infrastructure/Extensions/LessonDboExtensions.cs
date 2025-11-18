using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class LessonDboExtensions
{
    public static Lesson ToLesson(this LessonDbo lessonNumber, IMapper mapper)
    {
        return mapper.Map<Lesson>(lessonNumber);
    }
    
    public static List<Lesson> ToLesson(this ICollection<LessonDbo> lessonNumbers, IMapper mapper)
    {
        return mapper.Map<List<Lesson>>(lessonNumbers);
    }
    
    public static LessonDbo ToLessonDbo(this Lesson lessonNumber, IMapper mapper)
    {
        return mapper.Map<LessonDbo>(lessonNumber);
    }
    
    public static List<LessonDbo> ToLessonDbo(this ICollection<Lesson> lessonNumbers, IMapper mapper)
    {
        return mapper.Map<List<LessonDbo>>(lessonNumbers);
    }
}