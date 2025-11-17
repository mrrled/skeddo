using Application.DtoModels;
using Domain.Models;

namespace Application.Services;

public interface IService
{
    public Task<List<DtoClassroom>> FetchClassroomsFromBackendAsync();
    public Task<List<DtoSchedule>> FetchSchedulesFromBackendAsync();
    public Task<List<DtoSchoolSubject>> FetchSchoolSubjectsFromBackendAsync();
    public Task<List<DtoStudyGroup>> FetchStudyGroupsFromBackendAsync();
    public Task<List<DtoTeacher>> FetchTeachersFromBackendAsync();
    public Task<List<DtoLessonNumber>> GetLessonNumbersByScheduleId(int scheduleId);
    public Task<List<DtoLesson>> GetLessonsByScheduleId(int scheduleId);
    public Task<DtoTeacher> GetTeacherById(int id);
    public Task AddTeacher(DtoTeacher teacherDto);
    public Task AddClassroom(DtoClassroom classroomDto);
    public Task AddStudyGroup(DtoStudyGroup studyGroupDto);
    public Task AddLessonNumber(DtoLessonNumber lessonNumberDto, int scheduleId);
    public Task AddSchoolSubject(DtoSchoolSubject schoolSubjectDto);
    public Task AddSchedule(DtoSchedule scheduleDto);
    public Task EditTeacher(DtoTeacher teacherDto);
    public Task EditClassroom(DtoClassroom oldClassroomDto, DtoClassroom newClassroomDto);
    public Task EditStudyGroup(DtoStudyGroup oldStudyGroupDto, DtoStudyGroup newStudyGroupDto);
    public Task EditLessonNumber(DtoLessonNumber oldLessonNumberDto, DtoLessonNumber newLessonNumberDto);
    public Task EditSchedule(DtoSchedule oldScheduleDto, DtoSchedule newScheduleDto);
    public Task EditSchoolSubject(DtoSchoolSubject oldSubjectDto, DtoSchoolSubject newSubjectDto);
    public Task DeleteTeacher(DtoTeacher teacherDto);
    public Task DeleteClassroom(DtoClassroom classroomDto);
    public Task DeleteStudyGroup(DtoStudyGroup studyGroupDto);
    public Task DeleteSchoolSubject(DtoSchoolSubject schoolSubjectDto);
    public Task DeleteSchedule(DtoSchedule scheduleDto);
}