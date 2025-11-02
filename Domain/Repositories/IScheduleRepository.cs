using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    List<Classroom> GetClassrooms();
    List<Schedule> GetSchedules();
    List<SchoolSubject> GetSchoolSubjects();
    List<StudyGroup> GetStudyGroups();
    List<Teacher> GetTeachers();
    List<TimeSlot> GetTimeSlots();
    List<Lesson> GetLessonsByScheduleId(int scheduleId);
    void AddTeacher(Teacher teacher);
    void AddLesson(Lesson lesson);
    void AddClassroom(Classroom classroom);
    void AddStudyGroup(StudyGroup studyGroup);
}