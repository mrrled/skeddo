using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    Task<List<Classroom>> GetClassroomListAsync();
    Task<List<Lesson>> GetLessonListAsync();
    Task<List<Schedule>> GetScheduleListAsync();
    Task<List<SchoolSubject>> GetSchoolSubjectListAsync();
    Task<List<StudyGroup>> GetStudyGroupListAsync();
    Task<List<Teacher>> GetTeacherListAsync();
    Task<List<TimeSlot>> GetTimeSlotListAsync();
    Task<List<Lesson>> GetLessonListByScheduleIdAsync(int scheduleId);
    Task AddTeacherAsync(Teacher teacher);
    Task AddLessonAsync(Lesson lesson);
    Task AddClassroomAsync(Classroom classroom);
    Task AddStudyGroupAsync(StudyGroup studyGroup);
}