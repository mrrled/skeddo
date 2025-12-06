using Application.DtoModels;

namespace Application;

public class EditLessonResult
{
    public LessonDraftDto? LessonDraft { get; private set; }
    public LessonDto? Lesson { get; private set; }
    public bool IsDraft { get; set; } 
    
    public static EditLessonResult Success(LessonDto lesson) => 
        new() { Lesson = lesson, IsDraft = false };

    public static EditLessonResult Downgraded(LessonDraftDto lessonDraft) => 
        new() { LessonDraft = lessonDraft, IsDraft = true}; 
}