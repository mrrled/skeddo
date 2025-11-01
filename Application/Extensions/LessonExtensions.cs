using Application.UIModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class LessonExtensions
{
    public static LessonDto ToLessonDto(this Lesson lesson, IMapper mapper)
    {
        return mapper.Map<LessonDto>(lesson);
    }
    
    public static List<LessonDto> ToLessonDto(this List<Lesson> lessons, IMapper mapper)
    {
        return mapper.Map<List<LessonDto>>(lessons);
    }
}