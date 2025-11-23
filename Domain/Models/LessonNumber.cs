namespace Domain.Models;

public record LessonNumber(int Number, string? Time)
{
    public int Number { get; } = Number;
    public string? Time { get;} = Time;
}