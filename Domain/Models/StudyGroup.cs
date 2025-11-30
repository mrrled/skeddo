namespace Domain.Models;

public record StudyGroup(string Name)
{
    public static StudyGroup CreateStudyGroup(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudyGroup(name);
    }
}