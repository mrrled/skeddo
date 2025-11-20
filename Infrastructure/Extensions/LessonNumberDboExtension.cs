using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions;

public static class LessonNumberDboExtension
{
    public static LessonNumber ToLessonNumber(this LessonNumberDbo lesson)
    {
        return DboMapper.ToLessonNumber(lesson);
    }
    
    public static List<LessonNumber> ToLessonNumber(this ICollection<LessonNumberDbo> lessons)
    {
        return DboMapper.ToLessonNumber(lessons);
    }
    
    public static LessonNumberDbo ToLessonNumberDbo(this LessonNumber lesson)
    {
        return DboMapper.ToLessonNumberDbo(lesson);
    }
    
    public static List<LessonNumberDbo> ToLessonNumberDbo(this ICollection<LessonNumber> lessons)
    {
        return DboMapper.ToLessonNumberDbo(lessons);
    }
}