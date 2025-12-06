using Application.DtoMapping;
using Application.DtoModels;
using Domain.Models;

namespace Application.DtoExtensions;

public static class LessonDraftDtoExtensions
{
    public static LessonDraftDto ToLessonDraftDto(this LessonDraft lesson)
    {
        return DtoMapper.ToLessonDraftDto(lesson);
    }
    
    public static List<LessonDraftDto> ToLessonsDraftDto(this ICollection<LessonDraft> lessons)
    {
        return DtoMapper.ToLessonDraftDto(lessons);
    }
}