namespace Domain.Models;

public record StudySubgroup
{
    public StudyGroup StudyGroup { get; private set; }
    public string Name { get; private set; }
    public static Result<StudySubgroup> CreateStudySubgroup(StudyGroup studySubgroup, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<StudySubgroup>.Failure("Название учебной подгруппы не может быть пустым.");
        return Result<StudySubgroup>.Success(new StudySubgroup { StudyGroup = studySubgroup, Name = name });
    }
}