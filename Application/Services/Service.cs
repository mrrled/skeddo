using Application.Extensions;
using Application.DtoModels;
using AutoMapper;
using Domain;

namespace Application.Services;

public class Service : IService
{
    private readonly IScheduleRepository repository;
    private readonly IMapper mapper;

    public Service(IScheduleRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public async Task<List<DtoClassroom>> FetchClassroomsFromBackendAsync()
    {
        var classroomList = await repository.GetClassroomListAsync();
        return classroomList.ToClassroomDto(mapper);
    }

    public async Task<List<DtoLesson>> FetchLessonsFromBackendAsync()
    {
        var lessonList = await repository.GetLessonListAsync();
        return lessonList.ToLessonDto(mapper);
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

    public async Task<List<DtoTimeSlot>> FetchTimeSlotsFromBackendAsync()
    {
        var timeSlotList = await repository.GetTimeSlotListAsync();
        return timeSlotList.ToTimeSlotDto(mapper);
    }
}