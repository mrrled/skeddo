using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class LessonDtoExtensions
{
    public static LessonDto ToLessonDto(this Lesson lesson)
    {
        return DtoMapper.ToLessonDto(lesson);
    }
    
    public static List<LessonDto> ToLessonsDto(this ICollection<Lesson> lessons)
    {
        return DtoMapper.ToLessonDto(lessons);
    }
    
    public static Lesson ToLesson(this LessonDto lesson)
    {
        return DtoMapper.ToLesson(lesson);
    }
    
    public static List<Lesson> ToLessons(this ICollection<LessonDto> lessons)
    {
        return DtoMapper.ToLesson(lessons);
    }
}