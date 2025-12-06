namespace Domain.Models;

public class Classroom(int id, string name, string? description = null) : Entity<int>(id)
{
    public string Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public static Classroom CreateClassroom(int id, string? name, string? description)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new Classroom(id, name, description);
    }

    public void SetName(string? newName)
    {
        if (newName is null)
            throw new ArgumentNullException();
        Name = newName;
    }

    public void SetDescription(string? newDescription)
    {
        Description = newDescription;
    }
}