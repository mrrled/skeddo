namespace Domain.Models;

public record StudySubgroup
{
    public StudyGroup StudyGroup { get; private set; }
    public string Name { get; private set; }
    public static StudySubgroup CreateStudySubgroup(StudyGroup studySubgroup, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudySubgroup { StudyGroup = studySubgroup, Name = name };
    }
}