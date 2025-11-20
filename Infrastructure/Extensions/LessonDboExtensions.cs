using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class LessonDboExtensions
{
    public static Lesson ToLesson(this LessonDbo lessonNumber)
    {
        return DboMapper.ToLesson(lessonNumber);
    }
    
    public static List<Lesson> ToLesson(this ICollection<LessonDbo> lessonNumbers)
    {
        return DboMapper.ToLesson(lessonNumbers);
    }
    
    public static LessonDbo ToLessonDbo(this Lesson lessonNumber)
    {
        return DboMapper.ToLessonDbo(lessonNumber);
    }
    
    public static List<LessonDbo> ToLessonDbo(this ICollection<Lesson> lessonNumbers)
    {
        return DboMapper.ToLessonDbo(lessonNumbers);
    }
}