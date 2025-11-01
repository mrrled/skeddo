using Application.Extensions;
using Application.UIModels;
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
    
    // public List<TeacherDto> FetchClassroomsFromBackend() => repository.GetClassrooms().ToTeacherDto(mapper);
    // public List<TeacherDto> FetchLessonsFromBackend() => repository.GetLessons().ToTeacherDto(mapper);
    // public List<TeacherDto> FetchSchedulesFromBackend() => repository.GetSchedules().ToTeacherDto(mapper);
    // public List<TeacherDto> FetchSchoolSubjectsFromBackend() => repository.GetSchoolSubjects().ToTeacherDto(mapper);
    // public List<TeacherDto> FetchStudyGroupsFromBackend() => repository.GetStudyGroups().ToTeacherDto(mapper);
    public List<TeacherDto> FetchTeachersFromBackend() => repository.GetTeachers().ToTeacherDto(mapper);
    // public List<TeacherDto> FetchTimeSlotsFromBackend() => repository.GetTimeSlots().ToTeacherDto(mapper);
}