namespace Domain.Models;

public class SchoolSubject(Guid id, string name) : Entity<Guid>(id)
{
    public string Name { get; set; } = name;
    public static Result<SchoolSubject> CreateSchoolSubject(Guid id, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<SchoolSubject>.Failure("Название предмета не может быть пустым.");
        return Result<SchoolSubject>.Success(new SchoolSubject(id, name));
    }

    public Result SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Название предмета не может быть пустым.");
        Name = name;
        return Result.Success();
    }
}