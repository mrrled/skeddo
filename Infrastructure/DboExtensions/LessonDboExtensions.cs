using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.DboMapping;

namespace Infrastructure.DboExtensions;

public static class LessonDboExtensions
{
    public static Lesson ToLesson(this LessonDbo lessonNumber)
    {
        return DboMapper.ToLesson(lessonNumber);
    }
    
    public static List<Lesson> ToLessons(this ICollection<LessonDbo> lessonNumbers)
    {
        return DboMapper.ToLesson(lessonNumbers);
    }
    
    public static LessonDbo ToLessonDbo(this Lesson lessonNumber)
    {
        return DboMapper.ToLessonDbo(lessonNumber);
    }
    
    public static List<LessonDbo> ToLessonsDbo(this ICollection<Lesson> lessonNumbers)
    {
        return DboMapper.ToLessonDbo(lessonNumbers);
    }
}