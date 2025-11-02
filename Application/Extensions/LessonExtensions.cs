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
    
    public static DtoLesson ToLessonDto(this Lesson lesson, IMapper mapper,
        Action<IMappingOperationOptions<object, DtoLesson>> configure)
    {
        return mapper.Map(lesson, configure);
    }
    
    public static List<DtoLesson> ToLessonDto(this List<Lesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<DtoLesson>>(lessons);
    }
    
    public static List<DtoLesson> ToLessonDto(this List<Lesson> lessons, IMapper mapper,
        Action<IMappingOperationOptions<object, List<DtoLesson>>> configure)
    {
        return mapper.Map(lessons, configure);
    }
}