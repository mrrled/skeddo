namespace Application.DtoModels;
public class CreateLessonResult
{
    public LessonDraftDto? LessonDraft { get; private set; }
    public LessonDto? Lesson { get; private set; }
    public bool IsDraft { get; set; } 
    
    public static CreateLessonResult Success(LessonDto lesson) => 
        new() { Lesson = lesson, IsDraft = false };

    public static CreateLessonResult Downgraded(LessonDraftDto lessonDraft) => 
        new() { LessonDraft = lessonDraft, IsDraft = true}; 
}