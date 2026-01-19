namespace Domain.Models;

public class LessonDraft(
    Guid id,
    Guid scheduleId,
    SchoolSubject schoolSubject,
    LessonNumber? lessonNumber,
    Teacher? teacher,
    StudyGroup? studyGroup,
    Classroom? classroom,
    StudySubgroup? studySubgroup = null,
    string? comment = null) : Entity<Guid>(id)
{
    public Guid ScheduleId { get; private set; } = scheduleId;
    public SchoolSubject SchoolSubject { get; private set; } = schoolSubject;
    public LessonNumber? LessonNumber { get; private set; } = lessonNumber;
    public Teacher? Teacher { get; private set; } = teacher;
    public StudyGroup? StudyGroup { get; private set; } = studyGroup;
    public StudySubgroup? StudySubgroup { get; private set; } = studySubgroup;
    public Classroom? Classroom { get; private set; } = classroom;
    public string? Comment { get; private set; } = comment;

    public static LessonDraft CreateFromLesson(Lesson lesson)
    {
        return new LessonDraft(lesson.Id,
            lesson.ScheduleId,
            lesson.SchoolSubject,
            lesson.LessonNumber,
            lesson.Teacher,
            lesson.StudyGroup,
            lesson.Classroom,
            lesson.StudySubgroup,
            lesson.Comment);
    }
    
    public Result SetSchoolSubject(SchoolSubject? subject)
    {
        if (subject is null)
            return Result.Failure("Урок не может быть без предмета");
        SchoolSubject = subject;
        return Result.Success();
    }

    public void SetLessonNumber(LessonNumber? number)
    {
        LessonNumber = number;
    }

    public void SetTeacher(Teacher? teacher)
    {
        Teacher = teacher;
    }

    public void SetStudyGroup(StudyGroup? studyGroup)
    {
        StudyGroup = studyGroup;
    }
    
    public void SetClassroom(Classroom? classroom)
    {
        Classroom = classroom;
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