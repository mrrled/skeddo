using Application.DtoModels;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class LessonNumberExtensions
{
    public static DtoLessonNumber ToLessonNumberDto(this LessonNumber dtoLessonNumber, IMapper mapper)
    {
        return mapper.Map<DtoLessonNumber>(dtoLessonNumber);
    }
    public static List<DtoLessonNumber> ToLessonNumberDto(this ICollection<LessonNumber> dtoLessonNumber, IMapper mapper)
    {
        return mapper.Map<List<DtoLessonNumber>>(dtoLessonNumber);
    }
    
    public static LessonNumber ToLessonNumber(this DtoLessonNumber dtoLessonNumber, IMapper mapper)
    {
        return mapper.Map<LessonNumber>(dtoLessonNumber);
    }

    public static List<LessonNumber> ToLessonNumber(this ICollection<DtoLessonNumber> dtoLessonNumber, IMapper mapper)
    {
        return mapper.Map<List<LessonNumber>>(dtoLessonNumber);
    }

}