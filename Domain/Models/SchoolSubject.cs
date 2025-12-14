namespace Domain.Models;

public class SchoolSubject(Guid id, string name) : Entity<Guid>(id)
{
    public string Name { get; set; } = name;
    public static SchoolSubject CreateSchoolSubject(Guid id, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new SchoolSubject(id, name);
    }

    public void SetName(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        Name = name;
    }
}