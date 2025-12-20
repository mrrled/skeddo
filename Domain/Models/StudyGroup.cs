namespace Domain.Models;

public class StudyGroup(Guid id, Guid scheduleId ,string name, List<StudySubgroup> studySubgroups) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public Guid ScheduleId { get; set; } = scheduleId;
    public List<StudySubgroup> StudySubgroups => studySubgroups;

    public static Result<StudyGroup> CreateStudyGroup(Guid id, Guid scheduleId, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<StudyGroup>.Failure("Название учебной группы не может быть пустым.");
        return Result<StudyGroup>.Success(new StudyGroup(id, scheduleId, name, new()));
    }
    
    public Result SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Название учебной группы не может быть пустым.");
        Name = name;
        return Result.Success();
    }
}