namespace Application.DtoModels;

public class LessonNumberDto : IComparable<LessonNumberDto>
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string? Time { get; set; } = string.Empty;

    public int CompareTo(LessonNumberDto? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }
}