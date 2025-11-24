using Application.Extensions;
using Application.DtoModels;
using Application.Mapping;
using AutoMapper;
using Domain;
using Domain.Models;

namespace Application.Services;

public class Service : IService
{
    private readonly IScheduleRepository repository;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public Service(IScheduleRepository repository, IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<List<ClassroomDto>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await repository.GetClassroomListAsync();
        return classroomList.ToClassroomDto();
    }

    public async Task<List<ScheduleDto>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await repository.GetScheduleListAsync();
        return scheduleList.ToScheduleDto();
    }

    public async Task<List<SchoolSubjectDto>> FetchSchoolSubjectsFromBackendAsync()
    {
        var schoolSubjectList = await repository.GetSchoolSubjectListAsync();
        return schoolSubjectList.ToSchoolSubjectDto();
    }

    public async Task<List<StudyGroupDto>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await repository.GetStudyGroupListAsync();
        return studyGroupList.ToStudyGroupDto();
    }

    public async Task<List<TeacherDto>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await repository.GetTeacherListAsync();
        return teacherList.ToTeacherDto();
    }

    public async Task<List<LessonNumberDto>> GetLessonNumbersByScheduleId(int scheduleId)
    {
        var lessonNumbers = await repository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumberDto();
    }

    public async Task<List<LessonDto>> GetLessonsByScheduleId(int scheduleId)
    {
        var lessonList = await repository.GetLessonsByScheduleIdAsync(scheduleId);
        return lessonList.ToLessonDto();
    }

    public async Task<TeacherDto> GetTeacherById(int id)
    {
        var teacher = await repository.GetTeacherByIdAsync(id);
        return teacher.ToTeacherDto();
    }

    public async Task AddTeacher(TeacherDto teacherDto)
    {
        var teacher = Schedule.CreateTeacher(teacherDto.Id, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, teacherDto.SchoolSubjects, teacherDto.StudyGroups);
        await repository.AddAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.AddAsync(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.AddAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.AddAsync(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await repository.AddAsync(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddSchedule(ScheduleDto scheduleDto)
    {
        var schedule = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await repository.AddAsync(schedule);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddLesson(LessonDto lessonDto, int scheduleId)
    {
        Teacher? teacher = null;
        var schedule = await repository.GetScheduleByIdAsync(scheduleId);
        var teacherDto = lessonDto.Teacher;
        if (teacherDto is not null)
            teacher = Schedule.CreateTeacher(
                teacherDto.Id,
                teacherDto.Name,
                teacherDto.Surname,
                teacherDto.Patronymic,
                teacherDto.SchoolSubjects,
                teacherDto.StudyGroups);
        var lesson = schedule.AddLesson(
            lessonDto.Id,
            lessonDto.Subject?.Name,
            lessonDto.LessonNumber.Number,
            teacher,
            lessonDto.StudyGroup?.Name,
            lessonDto.Classroom?.Name,
            lessonDto.Classroom?.Description
        );
        await repository.AddAsync(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLesson(LessonDto lessonDto, int scheduleId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonDto.Id, scheduleId);
        lesson.Update(
            lessonDto.Subject?.ToSchoolSubject(),
            lessonDto.LessonNumber?.ToLessonNumber(),
            lessonDto.Teacher?.ToTeacher(),
            lessonDto.StudyGroup?.ToStudyGroup(),
            lessonDto.Classroom?.ToClassroom(),
            lessonDto.Comment
        );
        await repository.UpdateAsync(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditTeacher(TeacherDto teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        teacher.Update(teacherDto.Name, teacherDto.Surname, teacherDto.Patronymic,
            teacherDto.SchoolSubjects,
            teacherDto.StudyGroups);
        await repository.UpdateAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditClassroom(ClassroomDto oldClassroomDto, ClassroomDto newClassroomDto)
    {
        var oldClassroom = Schedule.CreateClassroom(oldClassroomDto.Name, oldClassroomDto.Description);
        var newClassroom = Schedule.CreateClassroom(newClassroomDto.Name, newClassroomDto.Description);
        await repository.UpdateAsync(oldClassroom, newClassroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudyGroup(StudyGroupDto oldStudyGroupDto, StudyGroupDto newStudyGroupDto)
    {
        var oldStudyGroup = Schedule.CreateStudyGroup(oldStudyGroupDto.Name);
        var newStudyGroup = Schedule.CreateStudyGroup(newStudyGroupDto.Name);
        await repository.UpdateAsync(oldStudyGroup, newStudyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLessonNumber(LessonNumberDto oldLessonNumberDto, LessonNumberDto newLessonNumberDto,
        int scheduleId)
    {
        var oldLessonNumber = Schedule.CreateLessonNumber(oldLessonNumberDto.Number, oldLessonNumberDto.Time);
        var newLessonNumber = Schedule.CreateLessonNumber(newLessonNumberDto.Number, newLessonNumberDto.Time);
        await repository.UpdateAsync(oldLessonNumber, newLessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchoolSubject(SchoolSubjectDto oldSubjectSchoolSubjectDto,
        SchoolSubjectDto newSubjectSchoolSubjectDto)
    {
        var oldSchoolSubject = Schedule.CreateSchoolSubject(oldSubjectSchoolSubjectDto.Name);
        var newSchoolSubject = Schedule.CreateSchoolSubject(newSubjectSchoolSubjectDto.Name);
        await repository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchedule(ScheduleDto oldScheduleDto, ScheduleDto newScheduleDto)
    {
        var oldSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        var newSchoolSubject = new Schedule(oldScheduleDto.Id, newScheduleDto.Name, []);
        await repository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLesson(LessonDto lessonDto, int scheduleId)
    {
        var lesson = await repository.GetLessonByIdAsync(lessonDto.Id, scheduleId);
        await repository.Delete(lesson, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteLessonNumber(LessonNumberDto lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.Number, lessonNumberDto.Time);
        await repository.Delete(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTeacher(TeacherDto teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        await repository.Delete(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteClassroom(ClassroomDto classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.Delete(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudyGroup(StudyGroupDto studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.Delete(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchoolSubject(SchoolSubjectDto schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchedule(ScheduleDto scheduleDto)
    {
        var schoolSubject = new Schedule(scheduleDto.Id, scheduleDto.Name, []);
        await repository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }
}