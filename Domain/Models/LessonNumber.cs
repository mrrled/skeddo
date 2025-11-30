namespace Domain.Models;

public record LessonNumber(int Number, string? Time)
{
    public int Number { get; } = Number;
    public string? Time { get;} = Time;
    public static LessonNumber CreateLessonNumber(int lessonNumber, string? time)
    {
        if (lessonNumber < 0)
            throw new ArgumentException();
        return new LessonNumber(lessonNumber, time);
    }
}