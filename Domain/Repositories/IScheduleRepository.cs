using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    public List<Classroom> GetClassrooms();
    public List<Lesson> GetLessons();
    public List<Schedule> GetSchedules();
    public List<SchoolSubject> GetSchoolSubjects();
    public List<StudyGroup> GetStudyGroups();
    public List<Teacher> GetTeachers();
    public List<TimeSlot> GetTimeSlots();
}