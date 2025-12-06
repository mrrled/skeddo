using Domain.Models;
using Infrastructure.DboMapping;
using Infrastructure.DboModels;

namespace Infrastructure.DboExtensions;

public static class LessonDraftDboExtensions
{
    public static LessonDraft ToLessonDraft(this LessonDraftDbo lessonDraftDbo)
    {
        return DboMapper.ToLessonDraft(lessonDraftDbo);
    }
    
    public static List<LessonDraft> ToLessonDrafts(this ICollection<LessonDraftDbo> lessonDraftDbos)
    {
        return DboMapper.ToLessonDraft(lessonDraftDbos);
    }
    
    public static LessonDraftDbo ToLessonDraftDbo(this LessonDraft lessonDraft)
    {
        return DboMapper.ToLessonDraftDbo(lessonDraft);
    }
    
    public static List<LessonDraftDbo> ToLessonsDraftDbos(this ICollection<LessonDraft> lessonDrafts)
    {
        return DboMapper.ToLessonDraftDbo(lessonDrafts);
    }
}