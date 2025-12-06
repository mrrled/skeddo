namespace Domain.Models;

public class SchoolSubject(int id, string name) : Entity<int>(id)
{
    public string Name { get; set; } = name;
    public static SchoolSubject CreateSchoolSubject(int id, string? name)
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