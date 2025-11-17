using Application.Extensions;
using Application.DtoModels;
using AutoMapper;
using Domain;
using Domain.Models;

namespace Application.Services;

public class Service : IService
{
    private readonly IScheduleRepository repository;
    private readonly IMapper mapper;
    private readonly IUnitOfWork unitOfWork;

    public Service(IScheduleRepository repository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.mapper = mapper;
        this.unitOfWork = unitOfWork;
    }

    public async Task<List<DtoClassroom>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await repository.GetClassroomListAsync();
        return classroomList.ToClassroomDto(mapper);
    }

    public async Task<List<DtoSchedule>> FetchSchedulesFromBackendAsync()
    {
        var scheduleList = await repository.GetScheduleListAsync();
        return scheduleList.ToScheduleDto(mapper);
    }

    public async Task<List<DtoSchoolSubject>> FetchSchoolSubjectsFromBackendAsync()
    {
        var schoolSubjectList = await repository.GetSchoolSubjectListAsync();
        return schoolSubjectList.ToSchoolSubjectDto(mapper);
    }

    public async Task<List<DtoStudyGroup>> FetchStudyGroupsFromBackendAsync()
    {
        var studyGroupList = await repository.GetStudyGroupListAsync();
        return studyGroupList.ToStudyGroupDto(mapper);
    }

    public async Task<List<DtoTeacher>> FetchTeachersFromBackendAsync()
    {
        var teacherList = await repository.GetTeacherListAsync();
        return teacherList.ToTeacherDto(mapper);
    }

    public async Task<List<DtoLessonNumber>> GetLessonNumbersByScheduleId(int scheduleId)
    {
        var lessonNumbers = await repository.GetLessonNumbersByScheduleIdAsync(scheduleId);
        return lessonNumbers.ToLessonNumberDto(mapper);
    }

    public async Task<List<DtoLesson>> GetLessonsByScheduleId(int scheduleId)
    {
        var lessonList = await repository.GetLessonsByScheduleIdAsync(scheduleId);
        return lessonList.ToLessonDto(mapper);
    }

    public async Task<DtoTeacher> GetTeacherById(int id)
    {
        var teacher = await repository.GetTeacherByIdAsync(id);
        return teacher.ToTeacherDto(mapper);
    }

    public async Task AddTeacher(DtoTeacher teacherDto)
    {
        var teacher = Schedule.CreateTeacher(teacherDto.Id, teacherDto.Name, teacherDto.Surname,
            teacherDto.Patronymic, teacherDto.Specialty, teacherDto.StudyGroups);
        await repository.AddAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddClassroom(DtoClassroom classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.AddAsync(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddStudyGroup(DtoStudyGroup studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.AddAsync(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddSchoolSubject(DtoSchoolSubject schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.AddAsync(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddLessonNumber(DtoLessonNumber lessonNumberDto, int scheduleId)
    {
        var lessonNumber = Schedule.CreateLessonNumber(lessonNumberDto.LessonNumber, lessonNumberDto.Time);
        await repository.AddAsync(lessonNumber, scheduleId);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddSchedule(DtoSchedule scheduleDto)
    {
        throw new NotImplementedException();
    }

    public async Task EditTeacher(DtoTeacher teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        teacher.Update(teacherDto.Name, teacherDto.Surname, teacherDto.Patronymic, 
            teacherDto.Specialty,
            teacherDto.StudyGroups);
        await repository.UpdateAsync(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditClassroom(DtoClassroom oldClassroomDto, DtoClassroom newClassroomDto)
    {
        var oldClassroom = Schedule.CreateClassroom(oldClassroomDto.Name, oldClassroomDto.Description);
        var newClassroom = Schedule.CreateClassroom(newClassroomDto.Name, newClassroomDto.Description);
        await repository.UpdateAsync(oldClassroom, newClassroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditStudyGroup(DtoStudyGroup oldStudyGroupDto, DtoStudyGroup newStudyGroupDto)
    {
        var oldStudyGroup = Schedule.CreateStudyGroup(oldStudyGroupDto.Name);
        var newStudyGroup = Schedule.CreateStudyGroup(newStudyGroupDto.Name);
        await repository.UpdateAsync(oldStudyGroup, newStudyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditLessonNumber(DtoLessonNumber oldLessonNumberDto, DtoLessonNumber newLessonNumberDto)
    {
        var oldLessonNumber = Schedule.CreateLessonNumber(oldLessonNumberDto.LessonNumber, oldLessonNumberDto.Time);
        var newLessonNumber = Schedule.CreateLessonNumber(newLessonNumberDto.LessonNumber, newLessonNumberDto.Time);
        await repository.UpdateAsync(oldLessonNumber, newLessonNumber);
        await unitOfWork.SaveChangesAsync();
    }
    
    public async Task EditSchoolSubject(DtoSchoolSubject oldSubjectDto, DtoSchoolSubject newSubjectDto)
    {
        var oldSchoolSubject = Schedule.CreateSchoolSubject(oldSubjectDto.Name);
        var newSchoolSubject = Schedule.CreateSchoolSubject(newSubjectDto.Name);
        await repository.UpdateAsync(oldSchoolSubject, newSchoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task EditSchedule(DtoSchedule oldScheduleDto, DtoSchedule newScheduleDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteTeacher(DtoTeacher teacherDto)
    {
        var teacher = await repository.GetTeacherByIdAsync(teacherDto.Id);
        await repository.Delete(teacher);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteClassroom(DtoClassroom classroomDto)
    {
        var classroom = Schedule.CreateClassroom(classroomDto.Name, classroomDto.Description);
        await repository.Delete(classroom);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStudyGroup(DtoStudyGroup studyGroupDto)
    {
        var studyGroup = Schedule.CreateStudyGroup(studyGroupDto.Name);
        await repository.Delete(studyGroup);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchoolSubject(DtoSchoolSubject schoolSubjectDto)
    {
        var schoolSubject = Schedule.CreateSchoolSubject(schoolSubjectDto.Name);
        await repository.Delete(schoolSubject);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSchedule(DtoSchedule scheduleDto)
    {
        throw new NotImplementedException();
    }
}