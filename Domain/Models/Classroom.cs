namespace Domain.Models;

public record Classroom(string Name, string? Description = null)
{
    public static Classroom CreateClassroom(string? name, string? description)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new Classroom(name, description);
    }
}