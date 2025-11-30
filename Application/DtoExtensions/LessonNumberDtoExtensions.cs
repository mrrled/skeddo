using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class LessonNumberDtoExtensions
{
    public static LessonNumberDto ToLessonNumberDto(this LessonNumber dtoLessonNumber)
    {
        var ln = DtoMapper.ToLessonNumberDto(dtoLessonNumber);
        return ln;
    }
    public static List<LessonNumberDto> ToLessonNumbersDto(this ICollection<LessonNumber> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumberDto(dtoLessonNumber);
    }
    
    public static LessonNumber ToLessonNumber(this LessonNumberDto lessonNumberDto)
    {
        var ln = DtoMapper.ToLessonNumber(lessonNumberDto);
        return ln;
    }

    public static List<LessonNumber> ToLessonNumbers(this ICollection<LessonNumberDto> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumber(dtoLessonNumber);
    }

}