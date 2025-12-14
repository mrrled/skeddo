namespace Domain.Models;

public class Lesson(
    int id,
    int scheduleId,
    SchoolSubject schoolSubject,
    LessonNumber lessonNumber,
    Teacher teacher,
    StudyGroup studyGroup,
    Classroom classroom,
    StudySubgroup? studySubgroup = null,
    string? comment = null,
    WarningType warningType = WarningType.Normal
) : Entity<int>(id)
{
    public int ScheduleId { get; private set; } = scheduleId;
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

    public void SetSchoolSubject(SchoolSubject? schoolSubject)
    {
        if (schoolSubject is null)
            throw new ArgumentNullException(nameof(schoolSubject));
        SchoolSubject = schoolSubject;
    }

    public void SetLessonNumber(LessonNumber? lessonNumber)
    {
        if (lessonNumber is null)
            throw new ArgumentNullException(nameof(lessonNumber));
        LessonNumber = lessonNumber;
    }

    public void SetTeacher(Teacher? teacher)
    {
        if (teacher is null)
            throw new ArgumentNullException(nameof(teacher));
        Teacher = teacher;
    }

    public void SetStudyGroup(StudyGroup? studyGroup)
    {
        if (studyGroup is null)
            throw new ArgumentNullException(nameof(studyGroup));
        StudyGroup = studyGroup;
    }

    public void SetClassroom(Classroom? classroom)
    {
        if (classroom is null)
            throw new ArgumentNullException(nameof(classroom));
        Classroom = classroom;
    }

    public void SetComment(string? comment)
    {
        Comment = comment;
    }
}