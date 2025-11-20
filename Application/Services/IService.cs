using Application.DtoModels;
using Domain.Models;

namespace Application.Services;

public interface IService
{
    public Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync();
    public Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync();
    public Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync();
    public Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync();
    public Task<List<TeacherDto>> FetchTeachersFromBackendAsync();
    public Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(int scheduleId);
    public Task<List<LessonDto>> GetLessonsByScheduleId(int scheduleId);
    public Task<TeacherDto> GetTeacherById(int id);
    public Task AddTeacher(TeacherDto teacherDto);
    public Task AddClassroom(ClassroomDto classroomDto);
    public Task AddStudyGroup(StudyGroupDto studyGroupDto);
    public Task AddLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId);
    public Task AddSchoolSubject(SchoolSubjectDto schoolSubjectDto);
    public Task AddSchedule(ScheduleDto scheduleDto);
    public Task AddLesson(LessonDto lessonDto, int scheduleId);
    public Task EditLesson(LessonDto lessonDto, int scheduleId);
    public Task EditTeacher(TeacherDto teacherDto);
    public Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto);
    public Task EditStudyGroup(StudyGroupDto oldStudyGroupDto, StudyGroupDto newStudyGroupDto);
    public Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto, int scheduleId);
    public Task EditSchedule(ScheduleDto oldScheduleDto, ScheduleDto newScheduleDto);
    public Task EditSchoolSubject(SchoolSubjectDto oldSubjectSchoolSubjectDto, SchoolSubjectDto newSubjectSchoolSubjectDto);
    public Task DeleteLesson(LessonDto lessonDto, int scheduleId);
    public Task DeleteLessonNumber(LessonDto numberLessonDto, int scheduleId);
    public Task DeleteTeacher(TeacherDto teacherDto);
    public Task DeleteClassroom(ClassroomDto classroomDto);
    public Task DeleteStudyGroup(StudyGroupDto studyGroupDto);
    public Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto);
    public Task DeleteSchedule(ScheduleDto scheduleDto);
}