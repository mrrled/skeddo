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
    
    public List<DtoClassroom> FetchClassroomsFromBackend() => repository.GetClassrooms().ToClassroomDto(mapper);
    public List<DtoLesson> FetchLessonsFromBackend() => repository.GetLessons().ToLessonDto(mapper);
    public List<DtoSchedule> FetchSchedulesFromBackend() => repository.GetSchedules().ToScheduleDto(mapper);
    public List<DtoSchoolSubject> FetchSchoolSubjectsFromBackend() => repository.GetSchoolSubjects().ToSchoolSubjectDto(mapper);
    public List<DtoStudyGroup> FetchStudyGroupsFromBackend() => repository.GetStudyGroups().ToStudyGroupDto(mapper);
    public List<DtoTeacher> FetchTeachersFromBackend() => repository.GetTeachers().ToTeacherDto(mapper);
    public List<DtoTimeSlot> FetchTimeSlotsFromBackend() => repository.GetTimeSlots().ToTimeSlotDto(mapper);
}