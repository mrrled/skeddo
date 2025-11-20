using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class LessonExtensions
{
    public static LessonDto ToLessonDto(this Lesson lesson)
    {
        return DtoMapper.ToLessonDto(lesson);
    }
    
    public static List<LessonDto> ToLessonDto(this ICollection<Lesson> lessons)
    {
        return DtoMapper.ToLessonDto(lessons);
    }
    
    public static Lesson ToLesson(this LessonDto lesson)
    {
        return DtoMapper.ToLesson(lesson);
    }
    
    public static List<Lesson> ToLesson(this ICollection<LessonDto> lessons)
    {
        return DtoMapper.ToLesson(lessons);
    }
}