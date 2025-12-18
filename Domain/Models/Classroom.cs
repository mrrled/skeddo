namespace Domain.Models;

public class Classroom(Guid id, string name, string? description = null) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public string? Description { get; private set; } = description;
    public static Result<Classroom> CreateClassroom(Guid id, string? name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Classroom>.Failure("Название класса не может быть пустым");
        return Result<Classroom>.Success(new Classroom(id, name, description));
    }

    public Result SetName(string? newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return Result.Failure("Название класса не может быть пустым");
        Name = newName;
        return Result.Success();
    }

    public void SetDescription(string? newDescription)
    {
        Description = newDescription;
    }
}