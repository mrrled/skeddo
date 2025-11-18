using AutoMapper;
using Domain.Models;
using Infrastructure.DboModels;

namespace Infrastructure.Extensions;

public static class LessonNumberDboExtension
{
    public static LessonNumber ToLessonNumber(this LessonNumberDbo lesson, IMapper mapper)
    {
        return mapper.Map<LessonNumber>(lesson);
    }
    
    public static List<LessonNumber> ToLessonNumber(this ICollection<LessonNumberDbo> lessons, IMapper mapper)
    {
        return mapper.Map<List<LessonNumber>>(lessons);
    }
    
    public static LessonNumberDbo ToLessonNumberDbo(this LessonNumber lesson, IMapper mapper)
    {
        return mapper.Map<LessonNumberDbo>(lesson);
    }
    
    public static List<LessonNumberDbo> ToLessonNumberDbo(this ICollection<LessonNumber> lessons, IMapper mapper)
    {
        return mapper.Map<List<LessonNumberDbo>>(lessons);
    }
}