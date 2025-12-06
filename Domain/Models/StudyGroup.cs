namespace Domain.Models;

public class StudyGroup(int id, string name) : Entity<int>(id)
{
    public string Name { get; private set; } = name;

    public static StudyGroup CreateStudyGroup(int id, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudyGroup(id, name);
    }
    
    public void SetName(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        Name = name;
    }
}