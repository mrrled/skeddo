using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboLessonExtensions
{
    public static Lesson ToLessonDto(this DboLesson dboLesson, IMapper mapper)
    {
        return mapper.Map<Lesson>(dboLesson);
    }
    
    public static List<Lesson> ToLessonDto(this List<DboLesson> dboLessons, IMapper mapper)
    {
        return mapper.Map<List<Lesson>>(dboLessons);
    }
}