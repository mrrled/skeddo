namespace Domain.Models;

public class StudyGroup(int id, string name, List<StudySubgroup> studySubgroups) : Entity<int>(id)
{
    public string Name { get; private set; } = name;
    public List<StudySubgroup> StudySubgroups => studySubgroups;

    public static StudyGroup CreateStudyGroup(int id, string? name)
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