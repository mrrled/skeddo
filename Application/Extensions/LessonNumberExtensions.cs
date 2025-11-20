using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain.Models;

namespace Application.Extensions;

public static class LessonNumberExtensions
{
    public static LessonNumberDto ToLessonNumberDto(this LessonNumber dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumberDto(dtoLessonNumber);
    }
    public static List<LessonNumberDto> ToLessonNumberDto(this ICollection<LessonNumber> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumberDto(dtoLessonNumber);
    }
    
    public static LessonNumber ToLessonNumber(this LessonNumberDto lessonNumberDto)
    {
        return DtoMapper.ToLessonNumber(lessonNumberDto);
    }

    public static List<LessonNumber> ToLessonNumber(this ICollection<LessonNumberDto> dtoLessonNumber)
    {
        return DtoMapper.ToLessonNumber(dtoLessonNumber);
    }

}