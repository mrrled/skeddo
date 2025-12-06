using Domain.Models;

namespace Domain;

public class LessonFactory : ILessonFactory
{
    public Result<Lesson> CreateFromDraft(LessonDraft lessonDraft)  //тут бизнес-проверки(может ли этот учитель вести этот урок и тд)
    {
        if (lessonDraft.StudyGroup is null)
            return Result<Lesson>.Failure("StudyGroup cannot be null");
        if (lessonDraft.Teacher is null)
            return Result<Lesson>.Failure("Teacher cannot be null");
        if (lessonDraft.LessonNumber is null)
            return Result<Lesson>.Failure("LessonNumber cannot be null");
        if (lessonDraft.Classroom is null)
            return Result<Lesson>.Failure("Classroom cannot be null");
        var lesson = new Lesson(lessonDraft.Id, lessonDraft.SchoolSubject, lessonDraft.LessonNumber, lessonDraft.Teacher, lessonDraft.StudyGroup, lessonDraft.Classroom, lessonDraft.Comment);
        return Result<Lesson>.Success(lesson);
    }
}