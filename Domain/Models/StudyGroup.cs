namespace Domain.Models;

public class StudyGroup(Guid id, string name, List<StudySubgroup> studySubgroups) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public List<StudySubgroup> StudySubgroups => studySubgroups;

    public static StudyGroup CreateStudyGroup(Guid id, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudyGroup(id, name, new());
    }
    
    public void SetName(string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        Name = name;
    }
}