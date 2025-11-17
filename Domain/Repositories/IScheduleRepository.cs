using Domain.Models;

namespace Domain;

public interface IScheduleRepository
{
    Task<List<Classroom>> GetClassroomListAsync();
    Task<List<Schedule>> GetScheduleListAsync();
    Task<List<SchoolSubject>> GetSchoolSubjectListAsync();
    Task<List<StudyGroup>> GetStudyGroupListAsync();
    Task<List<Teacher>> GetTeacherListAsync();
    Task<List<Lesson>> GetLessonsByScheduleIdAsync(int scheduleId);
    Task<List<LessonNumber>> GetLessonNumbersByScheduleIdAsync(int scheduleId);
    Task<Lesson> GetLessonByIdAsync(int id);
    Task<Teacher> GetTeacherByIdAsync(int id);
    Task<Schedule> GetScheduleByIdAsync(int scheduleId);
    Task AddAsync(Teacher teacher);
    Task AddAsync(Classroom classroom);
    Task AddAsync(StudyGroup studyGroup);
    Task AddAsync(SchoolSubject schoolSubject);
    Task AddAsync(LessonNumber lessonNumber, int scheduleId);
    Task UpdateAsync(Teacher teacher);
    Task UpdateAsync(Classroom oldClassroom, Classroom newClassroom);
    Task UpdateAsync(StudyGroup oldStudyGroup, StudyGroup newStudyGroup);
    Task UpdateAsync(SchoolSubject oldSchoolSubject, SchoolSubject newSchoolSubject);
    Task UpdateAsync(LessonNumber oldLessonNumber, LessonNumber newLessonNumber);   //можем поменять только время, но не номер
    Task Delete(Teacher teacher);
    Task Delete(Classroom classroom);
    Task Delete(StudyGroup studyGroup);
    Task Delete(SchoolSubject schoolSubject);
    Task Delete(LessonNumber lessonNumber);
}