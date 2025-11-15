using Application.DtoModels;
using Domain;
using Domain.Models;

namespace Application.Services;

public interface IService
{
    public Task<List<DtoClassroom>> FetchClassroomsFromBackendAsync();
    public Task<List<DtoLesson>> FetchLessonsFromBackendAsync();
    public Task<List<DtoSchedule>> FetchSchedulesFromBackendAsync();
    public Task<List<DtoSchoolSubject>> FetchSchoolSubjectsFromBackendAsync();
    public Task<List<DtoStudyGroup>> FetchStudyGroupsFromBackendAsync();
    public Task<List<DtoTeacher>> FetchTeachersFromBackendAsync();
    public Task<List<DtoTimeSlot>> FetchTimeSlotsFromBackendAsync();
}