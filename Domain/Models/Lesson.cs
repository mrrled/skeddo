namespace Domain.Models;

public class Lesson(
    Guid id,
    Guid scheduleId,
    SchoolSubject schoolSubject,
    LessonNumber lessonNumber,
    Teacher teacher,
    StudyGroup studyGroup,
    Classroom classroom,
    StudySubgroup? studySubgroup = null,
    string? comment = null,
    WarningType warningType = WarningType.Normal
) : Entity<Guid>(id)
{
    public Guid ScheduleId { get; private set; } = scheduleId;
    public SchoolSubject SchoolSubject { get; private set; } = schoolSubject;
    public LessonNumber LessonNumber { get; private set; } = lessonNumber;
    public Teacher Teacher { get; private set; } = teacher;
    public StudyGroup StudyGroup { get; private set; } = studyGroup;
    public StudySubgroup? StudySubgroup { get; private set; } = studySubgroup;
    public Classroom Classroom { get; private set; } = classroom;
    public string? Comment { get; private set; } = comment;
    public WarningType WarningType { get; private set; } = warningType;
    
    internal void SetWarningType(WarningType warningType)
    {
        WarningType = warningType;
    }

    public Result SetSchoolSubject(SchoolSubject? schoolSubject)
    {
        if (schoolSubject is null)
            return Result.Failure("Урок не может быть без предмета");
        SchoolSubject = schoolSubject;
        return Result.Success();
    }

    public Result SetLessonNumber(LessonNumber? lessonNumber)
    {
        if (lessonNumber is null)
            return Result.Failure("Урок не может быть без номера урока");
        LessonNumber = lessonNumber;
        return Result.Success();
    }

    public Result SetTeacher(Teacher? teacher)
    {
        if (teacher is null)
            return Result.Failure("Урок не может быть без учителя");
        Teacher = teacher;
        return Result.Success();
    }

    public Result SetStudyGroup(StudyGroup? studyGroup)
    {
        if (studyGroup is null)
            return Result.Failure("Урок не может быть без учебной группы");
        StudyGroup = studyGroup;
        return Result.Success();
    }
    
    public void SetStudySubgroup(StudySubgroup? subgroup)
    {
        StudySubgroup = subgroup;
    }

    public Result SetClassroom(Classroom? classroom)
    {
        if (classroom is null)
            return Result.Failure("Урок не может быть без аудитории");
        Classroom = classroom;
        return Result.Success();
    }

    public void SetStudySubgroup(StudySubgroup? studySubgroup)
    {
        StudySubgroup = studySubgroup;
    }

    public void SetComment(string? comment)
    {
        Comment = comment;
    }
}