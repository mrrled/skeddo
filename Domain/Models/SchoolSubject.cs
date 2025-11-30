namespace Domain.Models;

public record SchoolSubject(string Name)
{
    public static SchoolSubject CreateSchoolSubject(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new SchoolSubject(name);
    }
}