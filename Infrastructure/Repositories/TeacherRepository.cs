using Domain;
using Domain.Models;

namespace Infrastructure.Repositories;

public class TeacherRepository : IScheduleRepository
{
    public List<Classroom> GetClassrooms()
    {
        throw new NotImplementedException();
    }

    public List<Schedule> GetSchedules()
    {
        throw new NotImplementedException();
    }

    public List<SchoolSubject> GetSchoolSubjects()
    {
        throw new NotImplementedException();
    }

    public List<StudyGroup> GetStudyGroups()
    {
        throw new NotImplementedException();
    }

    public List<Teacher> GetTeachers()
    {
        return Teachers;
    }

    public List<TimeSlot> GetTimeSlots()
    {
        throw new NotImplementedException();
    }

    public List<Lesson> GetLessonsByScheduleId(int scheduleId)
    {
        throw new NotImplementedException();
    }

    public void AddTeacher(Teacher teacher)
    {
        throw new NotImplementedException();
    }

    public void AddLesson(Lesson lesson)
    {
        throw new NotImplementedException();
    }

    public void AddClassroom(Classroom classroom)
    {
        throw new NotImplementedException();
    }

    public void AddStudyGroup(StudyGroup studyGroup)
    {
        throw new NotImplementedException();
    }

    private static readonly List<Teacher> Teachers =
    [
        new Teacher(42,
            new FullName("Иван", "Иванов", "Иванович"),
            [new SchoolSubject("математика")],
            []),


        new Teacher(52,
            new FullName("Имя", "Фамилия", "Отчество"),
            [new SchoolSubject("оригами")],
            []),


        new Teacher(86,
            new FullName("Абоба", "Чиназес", "Чиловович"),
            [new SchoolSubject("пинает всякое разное")],
            [])
    ];
}