namespace Domain.Models;

public record LessonNumber(int Number, string? Time)
{
    public int Number { get; } = Number;
    public string? Time { get;} = Time;
    public static Result<LessonNumber> CreateLessonNumber(int lessonNumber, string? time)
    {
        if (lessonNumber < 0)
            return Result<LessonNumber>.Failure("Номер урока должен быть больше 0");
        return Result<LessonNumber>.Success(new LessonNumber(lessonNumber, time));
    }
}