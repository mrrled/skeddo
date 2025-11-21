using System.Threading.Tasks;
using Application.DtoModels;

namespace newUI.ViewModels.Lessons;

public class LessonCreationViewModel : ViewModelBase
{
    private readonly DtoLesson lesson;
    
    public LessonCreationViewModel()
    {
        lesson = new DtoLesson();
    }

    public void SetTeacher(DtoTeacher teacher) => lesson.Teacher = teacher;

    public void SetClassroom(DtoClassroom classroom) => lesson.Classroom = classroom;

    public void SetTimeSlot(int number) =>
        lesson.LessonNumber = new DtoLessonNumber()
        {
            Number = number
        };

    public void SetStudyGroup(DtoStudyGroup studyGroup) => lesson.StudyGroup = studyGroup;

    public void SetSubject(DtoSchoolSubject subject) => lesson.Subject = subject;

    public async Task<DtoLesson> CreateLesson()
    {
        return lesson;
    }
}