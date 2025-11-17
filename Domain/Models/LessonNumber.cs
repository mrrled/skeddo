namespace Domain.Models;

public class LessonNumber(int Number, string? Time)
{
    public int Number { get; } = Number;
    public string? Time { get;} = Time;
}