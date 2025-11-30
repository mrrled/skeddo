using System.Threading.Tasks;
using Application.DtoModels;

namespace newUI.ViewModels.Lessons;

public class LessonCreationViewModel : ViewModelBase
{
    private readonly LessonDto lesson;
    
    public LessonCreationViewModel()
    {
        lesson = new LessonDto();
    }

    public void SetTeacher(TeacherDto teacher) => lesson.Teacher = teacher;

    public void SetClassroom(ClassroomDto classroom) => lesson.Classroom = classroom;

    public void SetTimeSlot(int number) =>
        lesson.LessonNumber = new LessonNumberDto()
        {
            Number = number
        };

    public void SetStudyGroup(StudyGroupDto studyGroup) => lesson.StudyGroup = studyGroup;

    public void SetSubject(SchoolSubjectDto subject) => lesson.Subject = subject;

    public async Task<LessonDto> CreateLesson()
    {
        return lesson;
    }
}