using Domain.Models;

namespace Domain;

public interface ILessonFactory
{
    Result<Lesson> CreateFromDraft(LessonDraft lessonDraft);
}