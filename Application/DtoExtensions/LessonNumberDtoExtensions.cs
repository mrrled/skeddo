using Application.DtoModels;
using Application.DtoMapping;
using AutoMapper;
using Domain.Models;

namespace Application.DtoExtensions;

public static class LessonNumberDtoExtensions
{
    public static LessonNumberDto ToLessonNumberDto(this LessonNumber dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumberDto(dtoLessonNumber);
    }
    public static List<LessonNumberDto> ToLessonNumbersDto(this ICollection<LessonNumber> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumberDto(dtoLessonNumber);
    }
    
    public static LessonNumber ToLessonNumber(this LessonNumberDto lessonNumberDto)
    {
        return DtoMapper.ToLessonNumber(lessonNumberDto);
    }

    public static List<LessonNumber> ToLessonNumbers(this ICollection<LessonNumberDto> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumber(dtoLessonNumber);
    }

}