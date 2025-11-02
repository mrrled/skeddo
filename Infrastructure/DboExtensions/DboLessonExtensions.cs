using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class DboLessonExtensions
{
    public static Lesson ToLesson(this DboLesson dboLesson, IMapper mapper)
    {
        return mapper.Map<Lesson>(dboLesson);
    }
    
    public static Lesson ToLesson(this DboLesson dboLesson, IMapper mapper,
        Action<IMappingOperationOptions<object, Lesson>> configure)
    {
        return mapper.Map(dboLesson, configure);
    }
    
    public static List<Lesson> ToLesson(this List<DboLesson> dboLessons, IMapper mapper)
    {
        return mapper.Map<List<Lesson>>(dboLessons);
    }
    
    public static List<Lesson> ToLesson(this List<DboLesson> dboLessons, IMapper mapper,
        Action<IMappingOperationOptions<object, List<Lesson>>> configure)
    {
        return mapper.Map(dboLessons, configure);
    }
}