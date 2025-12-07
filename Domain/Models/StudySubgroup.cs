namespace Domain.Models;

public record StudySubgroup(StudyGroup Group, string Name)
{
    public static StudySubgroup CreateStudySubgroup(StudyGroup studySubgroup, string? name)
    {
        if (name is null)
            throw new ArgumentNullException();
        return new StudySubgroup(studySubgroup, name);
    }
}